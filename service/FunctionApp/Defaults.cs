using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.WebJobs.Host;
using SimpleInjector;

namespace FunctionApp
{
    public static class Defaults
    {
        public static void ApplyRequestHandlingDefaults(HttpRequestMessage request)
        {
            // Use our own JSON serializer settings everywhere.
            GlobalConfig.EnsureLoaded();
            foreach (var formatter in request.GetConfiguration().Formatters.OfType<JsonMediaTypeFormatter>())
                formatter.SerializerSettings = Constants.JsonSerializerSettings;

            // Always include full error details.
            request.GetRequestContext().IncludeErrorDetail = true;
        }
    }
}
