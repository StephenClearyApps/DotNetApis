using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;

namespace FunctionApp
{
    public static class ExpectedExceptionExtensions
    {
        public static HttpResponseMessage CreateErrorResponseWithLog(this HttpRequestMessage @this, ExpectedException exception, InMemoryLogger inMemoryLogger) =>
            @this.CreateErrorResponse(exception.HttpStatusCode, DetailedExceptionHandler.DetailExceptionsWithLog(@this, exception, inMemoryLogger));
    }
}
