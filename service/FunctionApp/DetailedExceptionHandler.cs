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
        private readonly ExceptionHandler _builtinExceptionHandler;

        public DetailedExceptionHandler(ExceptionHandler builtinExceptionHandler)
        {
            _builtinExceptionHandler = builtinExceptionHandler;
        }

        public override async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var exception = context.Exception is FunctionInvocationException && context.Exception.InnerException != null ? context.Exception.InnerException : context.Exception;

            // Attempt to capture a log from the in-memory logger.
            var details = new HttpError(exception, includeErrorDetail: true);
            var logger = context.Request.Properties.TryGetValue("testtest", out object loggerObject) ? loggerObject as InMemoryLogger : null;
            if (logger != null)
                details.Add("log", logger.Messages);

            // Attempt to extract the error object specified by the default exception handler.
            if (_builtinExceptionHandler != null)
                await _builtinExceptionHandler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
            var errorObject = await TryExtractObjectAsync(context);
            if (errorObject != null)
                details.Add("details", errorObject);

            context.Result = new ResponseMessageResult(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, details));
        }

        private static async Task<object> TryExtractObjectAsync(ExceptionHandlerContext context)
        {
            try
            {
                var responseMessageResult = context.Result as ResponseMessageResult;
                if (responseMessageResult == null)
                    return null;
                return await responseMessageResult.Response.Content.ReadAsAsync<object>();
            }
            catch
            {
                return null;
            }
        }
    }
}
