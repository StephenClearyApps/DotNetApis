using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using FunctionApp.Messages;
using Logic;
using Logic.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;
using Storage;

namespace FunctionApp
{
    public static class StatusFunction
    {
        [FunctionName("StatusFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/status")]HttpRequestMessage req,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            AmbientContext.Initialize(log, writer, req.IsLocal(), req.TryGetRequestId(), context.InvocationId);
            req.ApplyRequestHandlingDefaults(context);

            using (AsyncScopedLifestyle.BeginScope(GlobalConfig.Container))
            {
                var logger = GlobalConfig.Container.GetInstance<ILogger>();
                try
                {
                    await GlobalConfig.EnsureInitilizationCompleteAsync().ConfigureAwait(false);

                    var query = req.GetQueryNameValuePairs().ToList();
                    var jsonVersion = query.Required("jsonVersion", int.Parse);
                    var packageId = query.Required("packageId");
                    var packageVersion = query.Required("packageVersion");
                    var targetFramework = query.Required("targetFramework");
                    var timestamp = query.Required("timestamp", DateTimeOffset.Parse); // TODO: force invariant culture everywhere
                    logger.LogDebug("Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}, timestamp={timestamp}",
                        jsonVersion, packageId, packageVersion, targetFramework, timestamp);

                    if (jsonVersion < JsonFactory.Version)
                    {
                        logger.LogError("Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}", jsonVersion, JsonFactory.Version);
                        throw new ExpectedException((HttpStatusCode)422, "Application needs to update; refresh the page.");
                    }

                    // Normalize the user request (determine version and target framework if not specified).
                    var handler = GlobalConfig.Container.GetInstance<StatusRequestHandler>();
                    var (idver, target) = handler.NormalizeRequest(packageId, packageVersion, targetFramework);

                    var result = await handler.TryGetStatusAsync(idver, target, timestamp).ConfigureAwait(false);
                    if (result == null)
                        throw new ExpectedException(HttpStatusCode.NotFound, "Request status not found.");

                    return req.CreateResponse(HttpStatusCode.OK, new StatusResponseMessage
                    {
                        Status = result.Value.status,
                        LogUri = result.Value.logUri,
                    });
                }
                catch (ExpectedException ex)
                {
                    logger.LogInformation("Returning {httpStatusCode}: {errorMessage}", (int)ex.HttpStatusCode, ex.Message);
                    return req.CreateErrorResponseWithLog(ex);
                }
            }
        }
    }
}
