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
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp
{
    public sealed class DetailedExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var exception = context.Exception is FunctionInvocationException && context.Exception.InnerException != null ? context.Exception.InnerException : context.Exception;
            context.Result = new ResponseMessageResult(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, DetailExceptionsWithLog(context.Request, exception)));
        }

        public static HttpError DetailExceptionsWithLog(HttpRequestMessage message, Exception exception)
        {
            var result = new HttpError(exception, includeErrorDetail: true);

            // Attempt to capture a log from the in-memory logger.
            var logger = message.TryGetInMemoryLogger();
            if (logger != null)
                result.Add("log", logger.Messages);

            // Attempt to write the Application Insights operation id (Azure Functions invocation id).
            var operationId = message.TryGetExecutionContext()?.InvocationId;
            if (operationId != null)
                result.Add("operationId", operationId.Value);

            // Attempt to write the Azure Functions request id.
            var requestId = message.TryGetRequestId();
            if (requestId != null)
                result.Add("requestId", requestId);

            return result;
        }
    }
}
