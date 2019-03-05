using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.Logic.Messages;
using FunctionApp.CompositionRoot;
using FunctionApp.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        public static async Task Run(
            [QueueTrigger("generate")]string queueMessage,
            [Queue("generate-poison")] IAsyncCollector<CloudQueueMessage> generatePoisonQueue,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(context.FunctionAppDirectory, "local.settings.json"), optional: true)
                    .AddEnvironmentVariables()
                    .Build();
                GlobalConfig.Initialize();
                AmbientContext.JsonLoggerProvider = new JsonLoggerProvider();
                AmbientContext.OperationId = context.InvocationId;
                AmbientContext.ConfigurationRoot = config; // TODO: DI the options pattern
                AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(AmbientContext.JsonLoggerProvider);
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));
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