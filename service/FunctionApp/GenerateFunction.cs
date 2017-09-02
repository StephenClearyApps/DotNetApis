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
    public static class GenerateFunction
    {
        [FunctionName("GenerateFunction")]
        public static async Task Run([QueueTrigger("generate")]string queueMessage,
            [Queue("generate-poison")] IAsyncCollector<CloudQueueMessage> generatePoisonQueue,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            try
            {
                AmbientContext.Initialize(log, writer, context.InvocationId);
                using (AsyncScopedLifestyle.BeginScope(GlobalConfig.Container))
                {
                    var message = JsonConvert.DeserializeObject<GenerateRequestMessage>(queueMessage);
                    AmbientContext.ParentOperationId = message.OperationId;

                    await GlobalConfig.Container.GetInstance<GenerateHandler>().HandleAsync(message).ConfigureAwait(false);
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
