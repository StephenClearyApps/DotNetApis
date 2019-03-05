using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace FunctionApp
{
    public static class Helpers
    {
        public static string Optional(this IEnumerable<KeyValuePair<string, StringValues>> @this, string name) =>
            @this.FirstOrDefault(x => x.Key == name).Value;

        public static string Required(this IEnumerable<KeyValuePair<string, StringValues>> @this, string name)
        {
            var result = @this.Optional(name);
            if (result == null)
                throw new ExpectedException(StatusCodes.Status400BadRequest, $"Parameter {name} is required.");
            return result;
        }

        public static T Optional<T>(this IEnumerable<KeyValuePair<string, StringValues>> @this, string name, Func<string, T> convert)
        {
            var stringValue = @this.Required(name);
            if (stringValue == null)
                return default(T);
            return Convert(name, stringValue, convert);
        }

        public static T Required<T>(this IEnumerable<KeyValuePair<string, StringValues>> @this, string name, Func<string, T> convert)
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
                throw new ExpectedException(StatusCodes.Status400BadRequest, $"Could not convert parameter {name} to {typeof(T).Name}.", ex);
            }
        }

        public static IActionResult EnableCacheHeaders(this IActionResult response, TimeSpan time)
        {
            return new HeaderActionResult(response)
            {
                Headers =
                {
                    CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = time,
                    },
                    Expires = DateTimeOffset.UtcNow + time,
                },
            };
        }

        public static IActionResult WithLocationHeader(this IActionResult response, Uri location)
        {
            return new HeaderActionResult(response)
            {
                Headers =
                {
                    Location = location,
                },
            };
        }

        private sealed class HeaderActionResult : IActionResult
        {
            private readonly IActionResult _result;

            public HeaderActionResult(IActionResult result)
            {
                _result = result;
                HeaderDictionary = new HeaderDictionary();
                Headers = new ResponseHeaders(HeaderDictionary);
            }

            public IHeaderDictionary HeaderDictionary { get; }

            public ResponseHeaders Headers { get; }

            public async Task ExecuteResultAsync(ActionContext context)
            {
                foreach (var header in HeaderDictionary)
                    context.HttpContext.Response.Headers.Append(header.Key, header.Value);
                await _result.ExecuteResultAsync(context);
            }
        }

        public static IActionResult DetailExceptionResponse(Exception exception)
        {
            if (exception is FunctionInvocationException && exception.InnerException != null)
                exception = exception.InnerException;
            var httpStatusCode = exception is ExpectedException expected ? expected.HttpStatusCode : StatusCodes.Status500InternalServerError;
            var resultObject = ExceptionProblemDetails(exception);
            resultObject.Status = httpStatusCode;
            if (AmbientContext.InMemoryLoggerProvider != null)
                resultObject.Extensions.Add("log", AmbientContext.InMemoryLoggerProvider.Messages);
            if (AmbientContext.OperationId != Guid.Empty)
                resultObject.Extensions.Add("operationId", AmbientContext.OperationId);
            return new ObjectResult(resultObject) { StatusCode = httpStatusCode };
        }

        private static ProblemDetails ExceptionProblemDetails(Exception exception)
        {
            var result = new ProblemDetails
            {
                Title = exception.GetType().FullName,
                Detail = exception.Message,
                Extensions =
                {
                    { "stackTrace", exception.StackTrace.Split('\n').Select(x => x.TrimEnd('\r')).ToList() },
                },
            };
            if (exception is AggregateException aggregateException && aggregateException.InnerExceptions.Count > 0)
                result.Extensions.Add("nested", aggregateException.InnerExceptions.Select(ExceptionProblemDetails));
            else if (exception.InnerException != null)
                result.Extensions.Add("nested", new[] { ExceptionProblemDetails(exception.InnerException) });
            return result;
        }
    }
}
