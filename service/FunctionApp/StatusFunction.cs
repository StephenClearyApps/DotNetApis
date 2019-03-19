using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.Structure;
using FunctionApp.CompositionRoot;
using FunctionApp.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SimpleInjector.Lifestyles;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FunctionApp
{
    public sealed class StatusFunction
    {
        private readonly StatusRequestHandler _handler;
        private readonly ILogger<StatusFunction> _logger;

        public StatusFunction(StatusRequestHandler handler, ILoggerFactory loggerFactory)
        {
            _handler = handler;
            _logger = loggerFactory.CreateLogger<StatusFunction>();
        }

        public async Task<IActionResult> RunAsync(HttpRequest req)
        {
            try
            {
                var jsonVersion = req.Query.Required("jsonVersion", int.Parse);
                var packageId = req.Query.Required("packageId");
                var packageVersion = req.Query.Required("packageVersion");
                var targetFramework = req.Query.Required("targetFramework");
                _logger.RequestReceived(jsonVersion, packageId, packageVersion, targetFramework);

                if (jsonVersion < JsonFactory.Version)
                {
                    _logger.UpdateRequired(jsonVersion, JsonFactory.Version);
                    throw new ExpectedException(StatusCodes.Status422UnprocessableEntity, "Application needs to update; refresh the page.");
                }

                // Parse and normalize the user request.
                var (idver, target) = _handler.NormalizeRequest(packageId, packageVersion, targetFramework);

                var result = await _handler.TryGetStatusAsync(idver, target);
                if (result == null)
                    throw new ExpectedException(StatusCodes.Status404NotFound, "Request status not found.");

                return new JsonResult(new StatusResponseMessage
                {
                    Status = result.Value.Status,
                    LogUri = result.Value.LogUri,
                    JsonUri = result.Value.JsonUri,
                }, Constants.CommunicationJsonSerializerSettings);
            }
            catch (ExpectedException ex)
            {
                _logger.ReturningError((int)ex.HttpStatusCode, ex.Message);
                throw;
            }
        }

        [FunctionName("StatusFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/status")]HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.secrets.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();
                GlobalConfig.Initialize();
                AmbientContext.InMemoryLoggerProvider = new InMemoryLoggerProvider();
                AmbientContext.OperationId = context.InvocationId;
                AmbientContext.ConfigurationRoot = config; // TODO: DI the options pattern
                AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(AmbientContext.InMemoryLoggerProvider);
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));

                var container = await Containers.GetContainerForAsync<StatusFunction>();
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    return await container.GetInstance<StatusFunction>().RunAsync(req);
                }
            }
            catch (Exception ex)
            {
                return Helpers.DetailExceptionResponse(ex);
            }
        }
    }

    internal static partial class Logging
    {
        public static void RequestReceived(this ILogger<StatusFunction> logger, int jsonVersion, string packageId, string packageVersion, string targetFramework) =>
            Logger.Log(logger, 1, LogLevel.Debug, "Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}",
                jsonVersion, packageId, packageVersion, targetFramework, null);

        public static void UpdateRequired(this ILogger<StatusFunction> logger, int requestedJsonVersion, int currentJsonVersion) =>
            Logger.Log(logger, 2, LogLevel.Error, "Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}",
                requestedJsonVersion, currentJsonVersion, null);

        public static void ReturningError(this ILogger<StatusFunction> logger, int httpStatusCode, string errorMessage) =>
            Logger.Log(logger, 3, LogLevel.Debug, "Returning {httpStatusCode}: {errorMessage}", httpStatusCode, errorMessage, null);
    }
}