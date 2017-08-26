using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Common;

namespace FunctionApp
{
    public static class Helpers
    {
        public static string Optional(this IEnumerable<KeyValuePair<string, string>> @this, string name) => @this.FirstOrDefault(x => x.Key == name).Value;

        public static string Required(this IEnumerable<KeyValuePair<string, string>> @this, string name)
        {
            var result = @this.Optional(name);
            if (result == null)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Parameter {name} is required.");
            return result;
        }

        public static T Optional<T>(this IEnumerable<KeyValuePair<string, string>> @this, string name, Func<string, T> convert)
        {
            var stringValue = @this.Required(name);
            if (stringValue == null)
                return default(T);
            return Convert(name, stringValue, convert);
        }

        public static T Required<T>(this IEnumerable<KeyValuePair<string, string>> @this, string name, Func<string, T> convert)
        {
            var stringValue = @this.Required(name);
            return Convert(name, stringValue, convert);
        }

        private static T Convert<T>(string name, string value, Func<string, T> convert)
        {
            try
            {
                return convert(value);
            }
            catch (Exception ex)
            {
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not convert parameter {name} to {typeof(T).Name}.", ex);
            }
        }

        public static HttpResponseMessage EnableCacheHeaders(this HttpResponseMessage response, TimeSpan time)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = time,
            };
            if (response.Content == null)
                response.Content = new StringContent(string.Empty);
            response.Content.Headers.Expires = DateTimeOffset.UtcNow + time;
            return response;
        }

        public static HttpResponseMessage WithLocationHeader(this HttpResponseMessage response, Uri location)
        {
            response.Headers.Location = location;
            return response;
        }
    }
}
