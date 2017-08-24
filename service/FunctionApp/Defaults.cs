using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Common;
using Microsoft.Azure.WebJobs;

namespace FunctionApp
{
    public static class Defaults
    {
        private static string InMemoryLoggerKey { get; } = Guid.NewGuid().ToString("N");
        private static string ExecutionContextKey { get; } = Guid.NewGuid().ToString("N");

        public static void ApplyRequestHandlingDefaults(this HttpRequestMessage request, ExecutionContext context)
        {
            // Use our own JSON serializer settings everywhere.
            var config = request.GetConfiguration();
            GlobalConfig.EnsureJsonSerializerSettings();
            foreach (var formatter in config.Formatters.OfType<JsonMediaTypeFormatter>())
                formatter.SerializerSettings = Constants.JsonSerializerSettings;

            // Propagate error details in responses generated from exceptions.
            request.Properties.Add(InMemoryLoggerKey, AmbientContext.Loggers.OfType<InMemoryLogger>().First());
            request.Properties.Add(ExecutionContextKey, context);
            request.GetConfiguration().Services.Replace(typeof(IExceptionHandler), new DetailedExceptionHandler());
        }

        public static InMemoryLogger TryGetInMemoryLogger(this HttpRequestMessage request) =>
            request.Properties.TryGetValue(InMemoryLoggerKey, out object value) ? value as InMemoryLogger : null;

        public static ExecutionContext TryGetExecutionContext(this HttpRequestMessage request) =>
            request.Properties.TryGetValue(ExecutionContextKey, out object value) ? value as ExecutionContext : null;

        // Key is hardcoded just to avoid dependency on the whole Microsoft.Azure.WebJobs.Script package and all its dependencies.
        public static string TryGetRequestId(this HttpRequestMessage request) =>
            request.Properties.TryGetValue("MS_AzureFunctionsRequestID", out object value) ? value as string : null;
    }
}
