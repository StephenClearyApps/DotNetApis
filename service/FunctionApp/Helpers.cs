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
    }
}
