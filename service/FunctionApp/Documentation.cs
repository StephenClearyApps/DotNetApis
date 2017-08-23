using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Logic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SimpleInjector.Lifestyles;

namespace FunctionApp
{
    public static class Documentation
    {
        [FunctionName("Documentation")]
        public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/doc")]HttpRequestMessage req, TraceWriter log, ExecutionContext context)
        {
            AmbientContext.Initialize(Enumerables.Return<ILogger>(new InMemoryLogger(), new TraceWriterLogger(log)));
            req.ApplyRequestHandlingDefaults(log, context);
            return DoRun(req);
        }

        private static async Task<HttpResponseMessage> DoRun(HttpRequestMessage req)
        {
            using (AsyncScopedLifestyle.BeginScope(GlobalConfig.Container))
            {
                var logger = GlobalConfig.Container.GetInstance<ILogger>();
                try
                {
                    GlobalConfig.EnsureInitilizationComplete();

                    var query = req.GetQueryNameValuePairs().ToList();
                    var jsonVersion = query.Required("jsonVersion", int.Parse);
                    var packageId = query.Required("packageId");
                    var packageVersion = query.Optional("packageVersion");
                    var targetFramework = query.Optional("targetFramework");
                    logger.Trace($"Received request for jsonVersion={jsonVersion}, packageId=`{packageId}`, packageVersion=`{packageVersion}`, targetFramework=`{targetFramework}`");

                    if (jsonVersion < JsonFactory.Version)
                    {
                        logger.Trace($"Requested JSON version {jsonVersion} is old; current JSON version is {JsonFactory.Version}; returning 422");
                        return req.CreateResponse((HttpStatusCode)422, "Application needs to update; refresh the page.");
                    }

                    var handler = GlobalConfig.Container.GetInstance<DocRequestHandler>();
                    var result = await handler.GetDocAsync(packageId, packageVersion, targetFramework);

                    logger.Trace($"Success!");
                    return req.CreateResponse(HttpStatusCode.OK, "Hello " + result);
                }
                catch (ExpectedException ex)
                {
                    logger.Trace($"Returning {(int)ex.HttpStatusCode}: {ex.Message}");
                    return req.CreateErrorResponseWithLog(ex);
                }
            }
        }
    }
}
