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
using Microsoft.Extensions.Logging;
using SimpleInjector.Lifestyles;

namespace FunctionApp
{
    public static class Documentation
    {
        [FunctionName("Documentation")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/doc")]HttpRequestMessage req, ILogger log)
        {
            AmbientContext.Initialize(Enumerables.Return(new InMemoryLogger(), log));
            req.ApplyRequestHandlingDefaults(null, null);

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
                    logger.LogDebug("Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}",
                        jsonVersion, packageId, packageVersion, targetFramework);

                    if (jsonVersion < JsonFactory.Version)
                    {
                        logger.LogError("Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}", jsonVersion, JsonFactory.Version);
                        throw new ExpectedException((HttpStatusCode)422, "Application needs to update; refresh the page.");
                    }

                    var handler = GlobalConfig.Container.GetInstance<DocRequestHandler>();
                    var result = await handler.GetDocAsync(packageId, packageVersion, targetFramework);

                    logger.LogDebug("Success!");
                    return req.CreateResponse(HttpStatusCode.OK, "Hello " + result);
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
