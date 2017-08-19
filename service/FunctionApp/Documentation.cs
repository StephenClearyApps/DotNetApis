using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp
{
    public static class Documentation
    {
        [FunctionName("Documentation")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/doc")]HttpRequestMessage req, TraceWriter log)
        {
            Defaults.ApplyRequestHandlingDefaults(req);
            try
            {
                var logger = new Logger(log, "Doc API", Guid.NewGuid());

                var query = req.GetQueryNameValuePairs().ToList();
                var jsonVersion = query.Required("jsonVersion", int.Parse);
                var packageId = query.Required("packageId");
                var packageVersion = query.Optional("packageVersion");
                var targetFramework = query.Optional("targetFramework");
                logger.Trace($"Received request for {jsonVersion}, {packageId}.");

                return req.CreateResponse(HttpStatusCode.OK, "Hello " + packageId);
            }
            catch (ExpectedException ex)
            {
                return req.CreateErrorResponse(ex);
            }
        }
    }
}
