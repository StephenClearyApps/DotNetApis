using DotNetApis.Common;
using FunctionApp.CompositionRoot;
using FunctionApp.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace FunctionApp
{
    public sealed class NugetSearchFunction
    {
        // TODO: Factory
        private static readonly HttpClient NugetClient = new HttpClient();

        private readonly ILogger<NugetSearchFunction> _logger;

        public NugetSearchFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NugetSearchFunction>();
        }

        public async Task<IActionResult> RunAsync(HttpRequest req)
        {
            try
            {
                var q = req.Query.Optional("query") ?? "";
                var skip = req.Query.Optional("skip", int.Parse);
                var includePrerelease = req.Query.Optional("includePrerelease", bool.Parse);
                var includeUnlisted = req.Query.Optional("includeUnlisted", bool.Parse);

                _logger.RequestReceived(q, skip, includePrerelease, includeUnlisted);

                // Build NuGet API query
                var uri = new UriBuilder("http://www.nuget.org/api/v2/Search()");
                var args = new Dictionary<string, string>
                {
                    { "searchTerm", $"'{q.Replace("'", "''")}'" },
                    { "$skip", skip.ToString(CultureInfo.InvariantCulture) },
                    { "$top", "7" },
                    { "includePrerelease", includePrerelease.ToString(CultureInfo.InvariantCulture).ToLowerInvariant() }
                };
                if (includeUnlisted)
                    args["includeDelisted"] = "true";
                uri.Query = Nito.UniformResourceIdentifiers.Implementation.UriUtil.FormUrlEncode(args);
                _logger.SearchingNuget(uri.Uri);

                // Translate xml to json
                var result = await NugetClient.GetStringAsync(uri.Uri);
                var doc = XDocument.Parse(result);
                var atom = XNamespace.Get("http://www.w3.org/2005/Atom");
                var metadata = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                var dataservices = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices");
                var hits = doc.Root.Elements(atom + "entry").Select(entry => entry.Element(metadata + "properties"))
                    .Select(properties => new SearchResponseMessage.Hit
                    {
                        Id = properties.Element(dataservices + "Id").Value,
                        Version = properties.Element(dataservices + "NormalizedVersion").Value,
                        Title = properties.Element(dataservices + "Title").Value,
                        IconUrl = properties.Element(dataservices + "IconUrl").Value,
                        Summary = properties.Element(dataservices + "Summary").Value,
                        Description = properties.Element(dataservices + "Description").Value,
                        TotalDownloads = properties.Element(dataservices + "DownloadCount").Value
                    });

                return new OkObjectResult(new SearchResponseMessage { Hits = hits.ToList() });
            }
            catch (ExpectedException ex)
            {
                _logger.ReturningError(ex.HttpStatusCode, ex.Message);
                throw;
            }
        }

        [FunctionName("NugetSearch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/search")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(context.FunctionAppDirectory, "local.settings.json"), optional: true)
                    .AddEnvironmentVariables()
                    .Build();
                GlobalConfig.Initialize();
                AmbientContext.InMemoryLoggerProvider = new InMemoryLoggerProvider();
                AmbientContext.OperationId = context.InvocationId;
                AmbientContext.ConfigurationRoot = config; // TODO: DI the options pattern
                AsyncLocalLoggerFactory.LoggerFactory = new LoggerFactory();
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(AmbientContext.InMemoryLoggerProvider);
                AsyncLocalLoggerFactory.LoggerFactory.AddProvider(new ForwardingLoggerProvider(log));

                var container = await Containers.GetContainerForAsync<NugetSearchFunction>();
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    return await container.GetInstance<NugetSearchFunction>().RunAsync(req);
                }
            }
            catch (Exception ex)
            {
                return Helpers.DetailExceptionResponse(ex);
            }
        }
    }

    internal static partial class Logging
    {
        public static void RequestReceived(this ILogger<NugetSearchFunction> logger, string query, int skip, bool includePrerelease, bool includeUnlisted) =>
            Logger.Log(logger, 1, LogLevel.Debug, "Received request for query={query}, skip={skip}, includePrerelease={includePrerelease}, includeUnlisted={includeUnlisted}",
                query, skip, includePrerelease, includeUnlisted, null);

        public static void SearchingNuget(this ILogger<NugetSearchFunction> logger, Uri uri) =>
            Logger.Log(logger, 2, LogLevel.Debug, "Searching on NuGet: {uri}", uri, null);

        public static void ReturningError(this ILogger<NugetSearchFunction> logger, int httpStatusCode, string errorMessage) =>
            Logger.Log(logger, 3, LogLevel.Debug, "Returning {httpStatusCode}: {errorMessage}", httpStatusCode, errorMessage, null);
    }
}
