using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
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
