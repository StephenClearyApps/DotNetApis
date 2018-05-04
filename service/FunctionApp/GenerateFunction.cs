using System;
using DotNetApis.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using SimpleInjector.Lifestyles;
using System.Threading.Tasks;
using FunctionApp.Messages;
using DotNetApis.Logic;
using DotNetApis.Logic.Messages;
using FunctionApp.CompositionRoot;
using Newtonsoft.Json;

namespace FunctionApp
{
    public sealed class GenerateFunction
    {
        private readonly GenerateHandler _handler;
        private readonly ILoggerFactory _loggerFactory;

        public GenerateFunction(GenerateHandler handler, ILoggerFactory loggerFactory)
        {
            _handler = handler;
			_loggerFactory = loggerFactory;
        }

        public async Task RunAsync(string queueMessage)
        {
            var message = JsonConvert.DeserializeObject<GenerateRequestMessage>(queueMessage, Constants.CommunicationJsonSerializerSettings);
            AmbientContext.ParentOperationId = message.OperationId;
	        AsyncLocalAblyLoggerProvider.TryCreate(message.NormalizedPackageId + "/" + message.NormalizedPackageVersion + "/" + message.NormalizedFrameworkTarget, _loggerFactory);

            await _handler.HandleAsync(message);
        }

        [FunctionName("GenerateFunction")]
        public static async Task Run([QueueTrigger("generate")]string queueMessage,
            [Queue("generate-poison")] IAsyncCollector<CloudQueueMessage> generatePoisonQueue,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            try
            {
                GlobalConfig.Initialize();
                AmbientContext.InMemoryLoggerProvider = new InMemoryLoggerProvider();
                AmbientContext.OperationId = context.InvocationId;
	            AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
	            AsyncLocalLoggerFactory.LoggerFactory.AddProvider(AmbientContext.InMemoryLoggerProvider);
	            AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));
	            AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new TraceWriterLoggerProvider(writer));
				AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new AsyncLocalAblyLoggerProvider());

                var container = await Containers.GetContainerForAsync<GenerateFunction>();
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    await container.GetInstance<GenerateFunction>().RunAsync(queueMessage);
                }
            }
            catch (Exception ex)
            {
                await generatePoisonQueue.AddAsync(new CloudQueueMessage(JsonConvert.SerializeObject(new GenerateFailedMessage
                {
                    QueueMessage = queueMessage,
                    ExceptionType = ex.GetType().FullName,
                    ExceptionMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace,
                }, Constants.CommunicationJsonSerializerSettings)));
            }
        }
    }
}