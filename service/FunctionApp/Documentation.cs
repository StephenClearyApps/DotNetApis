using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using FunctionApp.Messages;
using Logic;
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
    public static class Documentation
    {
        [FunctionName("Documentation")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/doc")] HttpRequestMessage req,
            [Queue("generate")] IAsyncCollector<CloudQueueMessage> generateQueue,
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
                    var packageVersion = query.Optional("packageVersion");
                    var targetFramework = query.Optional("targetFramework");
                    logger.LogDebug("Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}",
                        jsonVersion, packageId, packageVersion, targetFramework);

                    if (jsonVersion < JsonFactory.Version)
                    {
                        logger.LogError("Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}", jsonVersion, JsonFactory.Version);
                        throw new ExpectedException((HttpStatusCode)422, "Application needs to update; refresh the page.");
                    }

                    // Normalize the user request (determine version and target framework if not specified).
                    var handler = GlobalConfig.Container.GetInstance<DocRequestHandler>();
                    var (idver, target) = await handler.NormalizeRequestAsync(packageId, packageVersion, targetFramework).ConfigureAwait(false);

                    // If the JSON is already there, then redirect the user to it.
                    var uri = await handler.TryGetExistingJsonUriAsync(idver, target).ConfigureAwait(false);
                    if (uri != null)
                    {
                        logger.LogDebug("Redirecting to {uri}", uri);
                        var cacheTime = packageVersion == null || targetFramework == null ? TimeSpan.FromDays(1) : TimeSpan.FromDays(7);
                        return req.CreateResponse(HttpStatusCode.TemporaryRedirect, new RedirectResponseMessage()).WithLocationHeader(uri).EnableCacheHeaders(cacheTime);
                    }

                    // Forward the request to the processing queue.
                    var timestamp = DateTimeOffset.UtcNow;
                    var message = JsonConvert.SerializeObject(new GenerateRequestMessage
                    {
                        Timestamp = timestamp,
                        NormalizedPackageId = idver.PackageId,
                        NormalizedPackageVersion = idver.Version.ToString(),
                        NormalizedFrameworkTarget = target.ToString(),
                    }, Constants.JsonSerializerSettings);
                    await generateQueue.AddAsync(new CloudQueueMessage(message)).ConfigureAwait(false);

                    logger.LogDebug("Enqueued request at {timestamp} for {idver} {target}: {message}", timestamp, idver, target, message);
                    return req.CreateResponse(HttpStatusCode.OK, new GenerateRequestQueuedResponseMessage
                    {
                        Timestamp = timestamp,
                        NormalizedPackageId = idver.PackageId,
                        NormalizedPackageVersion = idver.Version.ToString(),
                        NormalizedFrameworkTarget = target.ToString(),
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
