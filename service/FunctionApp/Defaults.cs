using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using DotNetApis.Common;
using Microsoft.Azure.WebJobs;

namespace FunctionApp
{
    public static class Defaults
    {
        public static void ApplyRequestHandlingDefaults(this HttpRequestMessage request, ExecutionContext context)
        {
            // Use our own JSON serializer settings everywhere.
            var config = request.GetConfiguration();
            foreach (var formatter in config.Formatters.OfType<JsonMediaTypeFormatter>())
                formatter.SerializerSettings = Constants.JsonSerializerSettings;

            // Propagate error details in responses generated from exceptions.
            request.GetConfiguration().Services.Replace(typeof(IExceptionHandler), new DetailedExceptionHandler());
        }

        // Key is hardcoded just to avoid dependency on the whole Microsoft.Azure.WebJobs.Script package and all its dependencies.
        public static string TryGetRequestId(this HttpRequestMessage request) =>
            request.Properties.TryGetValue("MS_AzureFunctionsRequestID", out object value) ? value as string : null;
    }
}
