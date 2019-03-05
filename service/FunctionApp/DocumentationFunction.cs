using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.Logic.Messages;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using DotNetApis.Structure;
using FunctionApp.CompositionRoot;
using FunctionApp.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FunctionApp
{
    public sealed class DocumentationFunction
    {
        private readonly ILogger<DocumentationFunction> _logger;
        private readonly DocRequestHandler _handler;
        private readonly IPackageJsonTable _packageJsonTable;

        public DocumentationFunction(ILoggerFactory loggerFactory, DocRequestHandler handler, IPackageJsonTable packageJsonTable)
        {
            _logger = loggerFactory.CreateLogger<DocumentationFunction>();
            _handler = handler;
            _packageJsonTable = packageJsonTable;
        }

        public async Task<IActionResult> RunAsync(HttpRequest req, IAsyncCollector<CloudQueueMessage> generateQueue)
        {
            try
            {
                var jsonVersion = req.Query.Required("jsonVersion", int.Parse);
                var packageId = req.Query.Required("packageId");
                var packageVersion = req.Query.Optional("packageVersion");
                var targetFramework = req.Query.Optional("targetFramework");
                _logger.RequestReceived(jsonVersion, packageId, packageVersion, targetFramework);

                if (jsonVersion < JsonFactory.Version)
                {
                    _logger.UpdateRequired(jsonVersion, JsonFactory.Version);
                    throw new ExpectedException(StatusCodes.Status422UnprocessableEntity, "Application needs to update; refresh the page.");
                }

                // Normalize the user request (determine version and target framework if not specified).
                var (idver, target) = await _handler.NormalizeRequestAsync(packageId, packageVersion, targetFramework);

                // If the JSON is already there, then redirect the user to it.
                var (jsonUri, logUri) = await _handler.TryGetExistingJsonAndLogUriAsync(idver, target);
                if (jsonUri != null)
                {
                    _logger.Redirecting(jsonUri);
                    var cacheTime = packageVersion == null || targetFramework == null ? TimeSpan.FromDays(1) : TimeSpan.FromDays(7);
                    return new OkObjectResult(new RedirectResponseMessage
                    {
                        NormalizedPackageId = idver.PackageId,
                        NormalizedPackageVersion = idver.Version.ToString(),
                        NormalizedFrameworkTarget = target.ToString(),
                        JsonUri = jsonUri,
                        LogUri = logUri,
                    }).EnableCacheHeaders(cacheTime);
                }

                // Make a note that it is in progress.
                var timestamp = DateTimeOffset.UtcNow;
                await _packageJsonTable.SetRecordAsync(idver, target, Status.Requested, null, null);

                // Forward the request to the processing queue.
                var message = JsonConvert.SerializeObject(new GenerateRequestMessage
                {
                    JsonVersion = jsonVersion,
                    NormalizedPackageId = idver.PackageId,
                    NormalizedPackageVersion = idver.Version.ToString(),
                    NormalizedFrameworkTarget = target.ToString(),
                }, Constants.CommunicationJsonSerializerSettings);
                await generateQueue.AddAsync(new CloudQueueMessage(message));

                _logger.EnqueuedRequest(timestamp, idver, target, message);
                return new OkObjectResult(new GenerateRequestQueuedResponseMessage
                {
                    NormalizedPackageId = idver.PackageId,
                    NormalizedPackageVersion = idver.Version.ToString(),
                    NormalizedFrameworkTarget = target.ToString(),
                });
            }
            catch (ExpectedException ex)
            {
                _logger.ReturningError(ex.HttpStatusCode, ex.Message);
                throw;
            }
        }

        [FunctionName("Documentation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/doc")] HttpRequest req,
            [Queue("generate")] IAsyncCollector<CloudQueueMessage> generateQueue,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(context.FunctionAppDirectory, "local.settings.json"), optional: true)
                    .AddEnvironmentVariables()
                    .Build();
                GlobalConfig.Initialize();
                AmbientContext.InMemoryLoggerProvider = new InMemoryLoggerProvider();
                AmbientContext.OperationId = context.InvocationId;
                AmbientContext.ConfigurationRoot = config; // TODO: DI the options pattern
                AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(AmbientContext.InMemoryLoggerProvider);
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));

                var container = await Containers.GetContainerForAsync<DocumentationFunction>();
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    return await container.GetInstance<DocumentationFunction>().RunAsync(req, generateQueue);
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
        public static void RequestReceived(this ILogger<DocumentationFunction> logger, int jsonVersion, string packageId, string packageVersion, string targetFramework) =>
            Logger.Log(logger, 1, LogLevel.Debug, "Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}",
                jsonVersion, packageId, packageVersion, targetFramework, null);

        public static void UpdateRequired(this ILogger<DocumentationFunction> logger, int requestedJsonVersion, int currentJsonVersion) =>
            Logger.Log(logger, 2, LogLevel.Error, "Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}",
                requestedJsonVersion, currentJsonVersion, null);

        public static void Redirecting(this ILogger<DocumentationFunction> logger, Uri uri) =>
            Logger.Log(logger, 3, LogLevel.Debug, "Redirecting to {uri}", uri, null);

        public static void EnqueuedRequest(this ILogger<DocumentationFunction> logger, DateTimeOffset timestamp, NugetPackageIdVersion idver, PlatformTarget target, string queueMessage) =>
            Logger.Log(logger, 4, LogLevel.Debug, "Enqueued request at {timestamp} for {idver} {target}: {queueMessage}", timestamp, idver, target, queueMessage, null);

        public static void ReturningError(this ILogger<DocumentationFunction> logger, int httpStatusCode, string errorMessage) =>
            Logger.Log(logger, 5, LogLevel.Debug, "Returning {httpStatusCode}: {errorMessage}", httpStatusCode, errorMessage, null);
    }
}
