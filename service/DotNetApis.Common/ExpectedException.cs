using System;
using System.Net;

namespace DotNetApis.Common
{
    public sealed class ExpectedException : Exception
    {
        public ExpectedException(int httpStatusCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public ExpectedException(HttpStatusCode httpStatusCode, string message, Exception innerException = null)
            : this((int)httpStatusCode, message, innerException)
        { }

        public int HttpStatusCode { get; }
    }
}
