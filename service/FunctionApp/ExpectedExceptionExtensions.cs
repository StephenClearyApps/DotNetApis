using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Common;

namespace FunctionApp
{
    public static class ExpectedExceptionExtensions
    {
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage @this, Logger logger, ExpectedException exception)
        {
            var details = new HttpError(exception, @this.ShouldIncludeErrorDetail())
            {
                { "log", logger.Messages },
            };
            return @this.CreateErrorResponse(exception.HttpStatusCode, details);
        }
    }
}
