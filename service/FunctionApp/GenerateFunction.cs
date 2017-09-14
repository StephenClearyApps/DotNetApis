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
using Newtonsoft.Json;

namespace FunctionApp
{
    public sealed class GenerateFunction
    {
        private readonly GenerateHandler _handler;

        public GenerateFunction(GenerateHandler handler)
        {
            _handler = handler;
        }

        public async Task RunAsync(string queueMessage)
        {
            var message = JsonConvert.DeserializeObject<GenerateRequestMessage>(queueMessage);
            AmbientContext.ParentOperationId = message.OperationId;

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
                AmbientContext.InMemoryLogger = new InMemoryLogger();
                AmbientContext.OperationId = context.InvocationId;
                AsyncLocalLogger.Logger = new CompositeLogger(Enumerables.Return(AmbientContext.InMemoryLogger, log, new TraceWriterLogger(writer))); // TODO: Ably

                var container = await CompositionRoot.GetContainerAsync();
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
                }, Constants.JsonSerializerSettings)));
            }
        }
    }
}