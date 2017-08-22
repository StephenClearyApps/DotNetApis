using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Common;

namespace FunctionApp
{
    public static class ExpectedExceptionExtensions
    {
        public static HttpResponseMessage CreateErrorResponseWithLog(this HttpRequestMessage @this, ExpectedException exception) =>
            @this.CreateErrorResponse(exception.HttpStatusCode, DetailedError(@this, exception));

        public static HttpResponseMessage CreateErrorResponseWithLog(this HttpRequestMessage @this, Exception exception) =>
            @this.CreateErrorResponse(HttpStatusCode.InternalServerError, DetailedError(@this, exception));

        private static HttpError DetailedError(HttpRequestMessage @this, Exception exception)
        {
            // Attempt to capture a log from the in-memory logger.
            var details = new HttpError(exception, @this.ShouldIncludeErrorDetail());
            var logger = AmbientContext.Loggers.OfType<InMemoryLogger>().FirstOrDefault();
            if (logger != null)
                details.Add("log", logger.Messages);
            return details;
        }
    }
}
