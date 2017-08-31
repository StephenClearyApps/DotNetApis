using System;
using Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using SimpleInjector.Lifestyles;
using System.Threading.Tasks;
using FunctionApp.Messages;
using Newtonsoft.Json;

namespace FunctionApp
{
    public static class GenerateFunction
    {
        [FunctionName("GenerateFunction")]
        public static async Task Run([QueueTrigger("generate")]string queueMessage,
            [Queue("generate")] IAsyncCollector<CloudQueueMessage> generatePoisonQueue,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            AmbientContext.Initialize(log, writer, context.InvocationId);
            using (AsyncScopedLifestyle.BeginScope(GlobalConfig.Container))
            {
                var logger = GlobalConfig.Container.GetInstance<ILogger>();
                try
                {
                    var message = JsonConvert.DeserializeObject<GenerateRequestMessage>(queueMessage);
                    AmbientContext.ParentOperationId = message.OperationId;

                    logger.LogDebug("Received message {message}", queueMessage);
                }
                catch (Exception ex)
                {
                    // Blob: container=log-yyyy-mm-dd, {id}/{ver}/{target}/{operationId}.txt
                    // Table: name=log-yyyy-mm-dd, PartitionKey=operationId, RowKey=operationId, requestId, id, ver, target, result, logblobpath

                    // TODO: Upload JSON error log to cloud in place of JSON documentation?
                    // Or in blob storage: {id}/{ver}/{target}/{datetimestamp}.txt?
                    // Also need some way to notify the UI. Table of backend requests: DateTimeStamp, operationId, requestId, id, ver, target, result, logblobpath
                    logger.LogCritical(0, ex, "Failed to process queue message {message}", queueMessage);
                    await generatePoisonQueue.AddAsync(new CloudQueueMessage(JsonConvert.SerializeObject(new GenerateFailedMessage
                    {
                        QueueMessage = queueMessage,
                    }, Constants.JsonSerializerSettings)));
                }
            }
        }
    }
}
