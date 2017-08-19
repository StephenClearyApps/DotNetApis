using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FunctionApp
{
    public sealed class ExpectedException : Exception
    {
        public ExpectedException(HttpStatusCode httpStatusCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }
        
        public HttpStatusCode HttpStatusCode { get; }
    }

    public static class ExpectedExceptionExtensions
    {
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage @this, ExpectedException exception) => @this.CreateErrorResponse(exception.HttpStatusCode, exception);
    }
}
