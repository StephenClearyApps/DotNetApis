using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Logic.Messages;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The response message returned by the HTTP trigger function if it is redirecting the client to the documentation JSON location.
    /// </summary>
    public sealed class RedirectResponseMessage : MessageBaseWithLog
    {
        /// <summary>
        /// The URI to JSON documentation.
        /// </summary>
        public Uri JsonUri { get; set; }

        /// <summary>
        /// The URI to the backend processing JSON log.
        /// </summary>
        public Uri LogUri { get; set; }

        /// <summary>
        /// The id of the package, lowercased.
        /// </summary>
        public string NormalizedPackageId { get; set; }

        /// <summary>
        /// The version of the package in a normalized form.
        /// </summary>
        public string NormalizedPackageVersion { get; set; }

        /// <summary>
        /// The standard NuGet short name for the target framework, lowercased.
        /// </summary>
        public string NormalizedFrameworkTarget { get; set; }
    }
}
