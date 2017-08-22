using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace FunctionApp
{
    public static class ExpectedExceptionExtensions
    {
        public static HttpResponseMessage CreateErrorResponseWithLog(this HttpRequestMessage @this, ExpectedException exception) =>
            @this.CreateErrorResponse(exception.HttpStatusCode, DetailedExceptionHandler.DetailExceptionsWithLog(@this, exception));
    }
}
