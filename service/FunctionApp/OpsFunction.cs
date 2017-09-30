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
        private readonly ILogger _logger;
        private readonly ProcessReferenceXmldocHandler _processReferenceXmldocHandler;

        public OpsFunction(ILogger logger, ProcessReferenceXmldocHandler processReferenceXmldocHandler)
        {
            _logger = logger;
            _processReferenceXmldocHandler = processReferenceXmldocHandler;
        }

        public async Task<HttpResponseMessage> RunAsync(HttpRequestMessage req)
        {
            var command = await req.Content.ReadAsAsync<OpsMessage>();
            _logger.LogDebug("Received command {command}", JsonConvert.SerializeObject(command));

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
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            GlobalConfig.Initialize();
            req.ApplyRequestHandlingDefaults(context);
            AmbientContext.OperationId = context.InvocationId;
            AmbientContext.RequestId = req.TryGetRequestId();
            AsyncLocalLogger.Logger = new CompositeLogger(Enumerables.Return(log, req.IsLocal() ? new TraceWriterLogger(writer) : null));

            var container = await Containers.GetContainerForAsync<OpsFunction>();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<OpsFunction>().RunAsync(req);
            }
        }
    }
}
