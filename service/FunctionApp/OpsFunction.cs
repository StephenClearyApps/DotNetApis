using DotNetApis.Common;
using DotNetApis.Logic;
using FunctionApp.CompositionRoot;
using FunctionApp.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FunctionApp
{
    public sealed class OpsFunction
    {
        private readonly ILogger<OpsFunction> _logger;
        private readonly ProcessReferenceXmldocHandler _processReferenceXmldocHandler;

        public OpsFunction(ILoggerFactory loggerFactory, ProcessReferenceXmldocHandler processReferenceXmldocHandler)
        {
            _logger = loggerFactory.CreateLogger<OpsFunction>();
            _processReferenceXmldocHandler = processReferenceXmldocHandler;
        }

        public async Task<IActionResult> RunAsync(HttpRequest req)
        {
            var command = new OpsMessage();//TODO: await req.Content.ReadAsAsync<OpsMessage>();
            _logger.ReceivedCommand(JsonConvert.SerializeObject(command, Constants.StorageJsonSerializerSettings));

            switch (command.Type)
            {
                case OpsMessageType.ProcessReferenceXmldoc:
                    await _processReferenceXmldocHandler.HandleAsync();
                    break;
                default:
                    throw new ExpectedException(StatusCodes.Status400BadRequest, $"Unknown type {command.Type}");
            }

            return new OkResult();
        }

        [FunctionName("OpsFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ops")]HttpRequest req,
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
                AmbientContext.OperationId = context.InvocationId;
                AmbientContext.ConfigurationRoot = config; // TODO: DI the options pattern
                AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));

                var container = await Containers.GetContainerForAsync<OpsFunction>();
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    return await container.GetInstance<OpsFunction>().RunAsync(req);
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
        public static void ReceivedCommand(this ILogger<OpsFunction> logger, string command) =>
            Logger.Log(logger, 1, LogLevel.Debug, "Received command {command}", command, null);
    }
}
