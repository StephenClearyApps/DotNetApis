using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using FunctionApp.CompositionRoot;
using FunctionApp.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;

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

        public async Task<HttpResponseMessage> RunAsync(HttpRequestMessage req)
        {
            var command = await req.Content.ReadAsAsync<OpsMessage>();
            _logger.ReceivedCommand(JsonConvert.SerializeObject(command, Constants.StorageJsonSerializerSettings));

            switch (command.Type)
            {
                case OpsMessageType.ProcessReferenceXmldoc:
                    await _processReferenceXmldocHandler.HandleAsync();
                    break;
                default:
                    throw new ExpectedException(HttpStatusCode.BadRequest, $"Unknown type {command.Type}");
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }

        [FunctionName("OpsFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "ops")]HttpRequestMessage req,
            ILogger log, ExecutionContext context)
        {
            GlobalConfig.Initialize();
            req.ApplyRequestHandlingDefaults(context);
            AmbientContext.OperationId = context.InvocationId;
            AmbientContext.RequestId = req.TryGetRequestId();
	        AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
	        AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));

            var container = await Containers.GetContainerForAsync<OpsFunction>();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<OpsFunction>().RunAsync(req);
            }
        }
    }

	internal static partial class Logging
	{
		public static void ReceivedCommand(this ILogger<OpsFunction> logger, string command) =>
			Logger.Log(logger, 1, LogLevel.Debug, "Received command {command}", command, null);
	}
}
