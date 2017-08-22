using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using Common;
using System.Net.Http;
using System.Web.Http.Filters;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp
{
    public sealed class DetailedExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var exception = context.Exception is FunctionInvocationException && context.Exception.InnerException != null ? context.Exception.InnerException : context.Exception;
            var details = DetailExceptionsWithLog(context.Request, exception);

            // Write a unique id to the log and include it in the response.
            var traceId = Guid.NewGuid().ToString("N");
            context.Request.TryGetTraceWriter().Error("traceId: " + traceId);
            details.Add("traceId", traceId);

            context.Result = new ResponseMessageResult(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, details));
        }

        public static HttpError DetailExceptionsWithLog(HttpRequestMessage message, Exception exception)
        {
            var result = new HttpError(exception, includeErrorDetail: true);

            // Attempt to capture a log from the in-memory logger.
            var logger = message.TryGetInMemoryLogger();
            if (logger != null)
                result.Add("log", logger.Messages);

            return result;
        }
    }
}
