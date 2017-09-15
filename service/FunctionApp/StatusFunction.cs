using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetApis.Common;
using FunctionApp.Messages;
using DotNetApis.Logic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SimpleInjector.Lifestyles;
using DotNetApis.Structure;

namespace FunctionApp
{
    public sealed class StatusFunction
    {
        private readonly StatusRequestHandler _handler;
        private readonly ILogger _logger;

        public StatusFunction(StatusRequestHandler handler, ILogger logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> RunAsync(HttpRequestMessage req)
        {
            try
            {
                var query = req.GetQueryNameValuePairs().ToList();
                var jsonVersion = query.Required("jsonVersion", int.Parse);
                var packageId = query.Required("packageId");
                var packageVersion = query.Required("packageVersion");
                var targetFramework = query.Required("targetFramework");
                var timestamp = query.Required("timestamp", DateTimeOffset.Parse);
                _logger.LogDebug("Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}, timestamp={timestamp}",
                    jsonVersion, packageId, packageVersion, targetFramework, timestamp);

                if (jsonVersion < JsonFactory.Version)
                {
                    _logger.LogError("Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}", jsonVersion, JsonFactory.Version);
                    throw new ExpectedException((HttpStatusCode)422, "Application needs to update; refresh the page.");
                }

                // Parse and normalize the user request.
                var (idver, target) = _handler.NormalizeRequest(packageId, packageVersion, targetFramework);

                var result = await _handler.TryGetStatusAsync(idver, target, timestamp);
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
                _logger.LogInformation("Returning {httpStatusCode}: {errorMessage}", (int)ex.HttpStatusCode, ex.Message);
                return req.CreateErrorResponseWithLog(ex);
            }
        }

        [FunctionName("StatusFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/status")]HttpRequestMessage req,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            GlobalConfig.Initialize();
            req.ApplyRequestHandlingDefaults(context);
            AmbientContext.InMemoryLogger = new InMemoryLogger();
            AmbientContext.OperationId = context.InvocationId;
            AmbientContext.RequestId = req.TryGetRequestId();
            AsyncLocalLogger.Logger = new CompositeLogger(Enumerables.Return(AmbientContext.InMemoryLogger, log, req.IsLocal() ? new TraceWriterLogger(writer) : null));

            var container = await CompositionRoot.GetContainerAsync();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<StatusFunction>().RunAsync(req);
            }
        }
    }
}