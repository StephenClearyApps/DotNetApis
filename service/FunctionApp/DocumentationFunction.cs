using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetApis.Common;
using FunctionApp.Messages;
using DotNetApis.Logic;
using DotNetApis.Logic.Messages;
using DotNetApis.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;
using DotNetApis.Structure;
using FunctionApp.CompositionRoot;

namespace FunctionApp
{
    public sealed class DocumentationFunction
    {
        private readonly ILogger _logger;
        private readonly DocRequestHandler _handler;
        private readonly IStatusTable _statusTable;

        public DocumentationFunction(ILogger logger, DocRequestHandler handler, IStatusTable statusTable)
        {
            _logger = logger;
            _handler = handler;
            _statusTable = statusTable;
        }

        public async Task<HttpResponseMessage> RunAsync(HttpRequestMessage req, IAsyncCollector<CloudQueueMessage> generateQueue)
        {
            try
            {
                var query = req.GetQueryNameValuePairs().ToList();
                var jsonVersion = query.Required("jsonVersion", int.Parse);
                var packageId = query.Required("packageId");
                var packageVersion = query.Optional("packageVersion");
                var targetFramework = query.Optional("targetFramework");
                _logger.LogDebug("Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}",
                    jsonVersion, packageId, packageVersion, targetFramework);

                if (jsonVersion < JsonFactory.Version)
                {
                    _logger.LogError("Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}", jsonVersion, JsonFactory.Version);
                    throw new ExpectedException((HttpStatusCode)422, "Application needs to update; refresh the page.");
                }

                // Normalize the user request (determine version and target framework if not specified).
                var (idver, target) = await _handler.NormalizeRequestAsync(packageId, packageVersion, targetFramework);

                // If the JSON is already there, then redirect the user to it.
                var uri = await _handler.TryGetExistingJsonUriAsync(idver, target);
                if (uri != null)
                {
                    _logger.LogDebug("Redirecting to {uri}", uri);
                    var cacheTime = packageVersion == null || targetFramework == null ? TimeSpan.FromDays(1) : TimeSpan.FromDays(7);
                    return req.CreateResponse(HttpStatusCode.TemporaryRedirect, new RedirectResponseMessage()).WithLocationHeader(uri).EnableCacheHeaders(cacheTime);
                }

                // Make a note that it is in progress.
                var timestamp = DateTimeOffset.UtcNow;
                await _statusTable.WriteStatusAsync(idver, target, timestamp, Status.Requested, null, null);

                // Forward the request to the processing queue.
                var message = JsonConvert.SerializeObject(new GenerateRequestMessage
                {
                    JsonVersion = jsonVersion,
                    Timestamp = timestamp,
                    NormalizedPackageId = idver.PackageId,
                    NormalizedPackageVersion = idver.Version.ToString(),
                    NormalizedFrameworkTarget = target.ToString(),
                }, Constants.JsonSerializerSettings);
                await generateQueue.AddAsync(new CloudQueueMessage(message));

                _logger.LogDebug("Enqueued request at {timestamp} for {idver} {target}: {message}", timestamp, idver, target, message);
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
                _logger.LogDebug("Returning {httpStatusCode}: {errorMessage}", (int)ex.HttpStatusCode, ex.Message);
                return req.CreateErrorResponseWithLog(ex);
            }
        }

        [FunctionName("Documentation")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/doc")] HttpRequestMessage req,
            [Queue("generate")] IAsyncCollector<CloudQueueMessage> generateQueue,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            GlobalConfig.Initialize();
            req.ApplyRequestHandlingDefaults(context);
            AmbientContext.InMemoryLogger = new InMemoryLogger();
            AmbientContext.OperationId = context.InvocationId;
            AmbientContext.RequestId = req.TryGetRequestId();
            AsyncLocalLogger.Logger = new CompositeLogger(Enumerables.Return(AmbientContext.InMemoryLogger, log, req.IsLocal() ? new TraceWriterLogger(writer) : null));

            var container = await Containers.GetContainerAsync();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<DocumentationFunction>().RunAsync(req, generateQueue);
            }
        }
    }
}
