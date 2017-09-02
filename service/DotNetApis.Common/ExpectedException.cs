using System;
using System.Net;

namespace DotNetApis.Common
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
}
