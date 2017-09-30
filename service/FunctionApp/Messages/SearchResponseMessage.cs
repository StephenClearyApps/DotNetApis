using System;
using System.Collections.Generic;
using DotNetApis.Logic.Messages;
using DotNetApis.Storage;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The message returned from the HTTP trigger function to search the NuGet API.
    /// </summary>
    public sealed class SearchResponseMessage : MessageBaseWithLog
    {
        public IReadOnlyList<Hit> Hits { get; set; }

        public sealed class Hit
        {
            public string Id { get; set; }
            public string Version { get; set; }
            public string Title { get; set; }
            public string IconUrl { get; set; }
            public string Summary { get; set; }
            public string Description { get; set; }
            public string TotalDownloads { get; set; }
        }
    }
}
