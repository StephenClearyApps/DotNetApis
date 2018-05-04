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
using FunctionApp.CompositionRoot;

namespace FunctionApp
{
    public sealed class StatusFunction
    {
        private readonly StatusRequestHandler _handler;
        private readonly ILogger _logger;

        public StatusFunction(StatusRequestHandler handler, ILoggerFactory loggerFactory)
        {
            _handler = handler;
            _logger = loggerFactory.CreateLogger<StatusFunction>();
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
                _logger.LogDebug("Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}",
                    jsonVersion, packageId, packageVersion, targetFramework);

                if (jsonVersion < JsonFactory.Version)
                {
                    _logger.LogError("Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}", jsonVersion, JsonFactory.Version);
                    throw new ExpectedException((HttpStatusCode)422, "Application needs to update; refresh the page.");
                }

                // Parse and normalize the user request.
                var (idver, target) = _handler.NormalizeRequest(packageId, packageVersion, targetFramework);

                var result = await _handler.TryGetStatusAsync(idver, target);
                if (result == null)
                    throw new ExpectedException(HttpStatusCode.NotFound, "Request status not found.");

                return req.CreateResponse(HttpStatusCode.OK, new StatusResponseMessage
                {
                    Status = result.Value.Status,
                    LogUri = result.Value.LogUri,
                    JsonUri = result.Value.JsonUri,
                });
            }
            catch (ExpectedException ex)
            {
                _logger.LogDebug("Returning {httpStatusCode}: {errorMessage}", (int)ex.HttpStatusCode, ex.Message);
                return req.CreateErrorResponseWithLog(ex);
            }
        }

        [FunctionName("StatusFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/status")]HttpRequestMessage req,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            GlobalConfig.Initialize();
            req.ApplyRequestHandlingDefaults(context);
	        AmbientContext.InMemoryLoggerProvider = new InMemoryLoggerProvider();
            AmbientContext.OperationId = context.InvocationId;
            AmbientContext.RequestId = req.TryGetRequestId();
	        AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
	        AsyncLocalLoggerFactory.LoggerFactory.AddProvider(AmbientContext.InMemoryLoggerProvider);
	        AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));
	        if (req.IsLocal())
		        AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new TraceWriterLoggerProvider(writer));

            var container = await Containers.GetContainerForAsync<StatusFunction>();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<StatusFunction>().RunAsync(req);
            }
        }
    }
}