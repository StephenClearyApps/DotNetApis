using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DotNetApis.Common;
using FunctionApp.Messages;
using DotNetApis.Logic;
using DotNetApis.Logic.Messages;
using DotNetApis.Nuget;
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
        private readonly ILogger<DocumentationFunction> _logger;
        private readonly DocRequestHandler _handler;
        private readonly IPackageJsonTable _packageJsonTable;

        public DocumentationFunction(ILoggerFactory loggerFactory, DocRequestHandler handler, IPackageJsonTable packageJsonTable)
        {
            _logger = loggerFactory.CreateLogger<DocumentationFunction>();
            _handler = handler;
            _packageJsonTable = packageJsonTable;
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
                _logger.RequestReceived(jsonVersion, packageId, packageVersion, targetFramework);

                if (jsonVersion < JsonFactory.Version)
                {
                    _logger.UpdateRequired(jsonVersion, JsonFactory.Version);
                    throw new ExpectedException((HttpStatusCode)422, "Application needs to update; refresh the page.");
                }

                // Normalize the user request (determine version and target framework if not specified).
                var (idver, target) = await _handler.NormalizeRequestAsync(packageId, packageVersion, targetFramework);

                // If the JSON is already there, then redirect the user to it.
                var (jsonUri, logUri) = await _handler.TryGetExistingJsonAndLogUriAsync(idver, target);
                if (jsonUri != null)
                {
                    _logger.Redirecting(jsonUri);
                    var cacheTime = packageVersion == null || targetFramework == null ? TimeSpan.FromDays(1) : TimeSpan.FromDays(7);
                    return req.CreateResponse(HttpStatusCode.OK, new RedirectResponseMessage
                    {
                        NormalizedPackageId = idver.PackageId,
                        NormalizedPackageVersion = idver.Version.ToString(),
                        NormalizedFrameworkTarget = target.ToString(),
                        JsonUri = jsonUri,
                        LogUri = logUri,
                    }).EnableCacheHeaders(cacheTime);
                }

                // Make a note that it is in progress.
                var timestamp = DateTimeOffset.UtcNow;
                await _packageJsonTable.SetRecordAsync(idver, target, Status.Requested, null, null);

                // Forward the request to the processing queue.
                var message = JsonConvert.SerializeObject(new GenerateRequestMessage
                {
                    JsonVersion = jsonVersion,
                    NormalizedPackageId = idver.PackageId,
                    NormalizedPackageVersion = idver.Version.ToString(),
                    NormalizedFrameworkTarget = target.ToString(),
                }, Constants.CommunicationJsonSerializerSettings);
                await generateQueue.AddAsync(new CloudQueueMessage(message));

                _logger.EnqueuedRequest(timestamp, idver, target, message);
                return req.CreateResponse(HttpStatusCode.Accepted, new GenerateRequestQueuedResponseMessage
                {
                    NormalizedPackageId = idver.PackageId,
                    NormalizedPackageVersion = idver.Version.ToString(),
                    NormalizedFrameworkTarget = target.ToString(),
                });
            }
            catch (ExpectedException ex)
            {
                _logger.ReturningError((int)ex.HttpStatusCode, ex.Message);
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
            AmbientContext.InMemoryLoggerProvider = new InMemoryLoggerProvider();
            AmbientContext.OperationId = context.InvocationId;
            AmbientContext.RequestId = req.TryGetRequestId();
	        AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
	        AsyncLocalLoggerFactory.LoggerFactory.AddProvider(AmbientContext.InMemoryLoggerProvider);
			AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));
			if (req.IsLocal())
				AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new TraceWriterLoggerProvider(writer));

            var container = await Containers.GetContainerForAsync<DocumentationFunction>();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<DocumentationFunction>().RunAsync(req, generateQueue);
            }
        }
    }

	internal static partial class Logging
	{
		public static void RequestReceived(this ILogger<DocumentationFunction> logger, int jsonVersion, string packageId, string packageVersion, string targetFramework) =>
			Logger.Log(logger, 1, LogLevel.Debug, "Received request for jsonVersion={jsonVersion}, packageId={packageId}, packageVersion={packageVersion}, targetFramework={targetFramework}",
				jsonVersion, packageId, packageVersion, targetFramework, null);

		public static void UpdateRequired(this ILogger<DocumentationFunction> logger, int requestedJsonVersion, int currentJsonVersion) =>
			Logger.Log(logger, 2, LogLevel.Error, "Requested JSON version {requestedJsonVersion} is old; current JSON version is {currentJsonVersion}",
				requestedJsonVersion, currentJsonVersion, null);

		public static void Redirecting(this ILogger<DocumentationFunction> logger, Uri uri) =>
			Logger.Log(logger, 3, LogLevel.Debug, "Redirecting to {uri}", uri, null);

		public static void EnqueuedRequest(this ILogger<DocumentationFunction> logger, DateTimeOffset timestamp, NugetPackageIdVersion idver, PlatformTarget target, string queueMessage) =>
			Logger.Log(logger, 4, LogLevel.Debug, "Enqueued request at {timestamp} for {idver} {target}: {queueMessage}", timestamp, idver, target, queueMessage, null);

		public static void ReturningError(this ILogger<DocumentationFunction> logger, int httpStatusCode, string errorMessage) =>
			Logger.Log(logger, 5, LogLevel.Debug, "Returning {httpStatusCode}: {errorMessage}", httpStatusCode, errorMessage, null);
	}
}
