using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using FunctionApp.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;

#if NO
namespace FunctionApp
{
    public static class OpsFunction
    {
        [FunctionName("OpsFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "ops")]HttpRequestMessage req,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            AmbientContext.InitializeForManualHttpTrigger(log, writer, req.IsLocal(), req.TryGetRequestId(), context.InvocationId);
            req.ApplyRequestHandlingDefaults(context);

            using (AsyncScopedLifestyle.BeginScope(GlobalConfig.Container))
            {
                GlobalConfig.Initialize();
                var logger = GlobalConfig.Container.GetInstance<ILogger>();

                var command = await req.Content.ReadAsAsync<OpsMessage>().ConfigureAwait(false);
                logger.LogDebug("Received command {command}", JsonConvert.SerializeObject(command));

                switch (command.Type)
                {
                    case OpsMessageType.ProcessReferenceXmldoc:
                        await GlobalConfig.Container.GetInstance<ProcessReferenceXmldocHandler>().HandleAsync().ConfigureAwait(false);
                        break;
                    default:
                        throw new ExpectedException(HttpStatusCode.BadRequest, $"Unknown type {command.Type}");
                }

                return req.CreateResponse(HttpStatusCode.OK);
            }
        }
    }
}
#endif