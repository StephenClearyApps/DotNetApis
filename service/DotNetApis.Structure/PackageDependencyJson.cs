using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a package dependency.
    /// </summary>
    public sealed class PackageDependencyJson
    {
        /// <summary>
        /// The title of the package.
        /// </summary>
        [JsonProperty("t")]
        public string Title { get; set; }

        /// <summary>
        /// The package id.
        /// </summary>
        [JsonProperty("i")]
        public string PackageId { get; set; }

        /// <summary>
        /// The resolved version of the package.
        /// </summary>
        [JsonProperty("v")]
        public string Version { get; set; }

        /// <summary>
        /// The version range of the package dependency, as requested by the primary package; this is <c>null</c> if this dependency is not an immediate dependency.
        /// </summary>
        [JsonProperty("q")]
        public string VersionRange { get; set; }

        /// <summary>
        /// Summary of the package. May be <c>null</c>.
        /// </summary>
        [JsonProperty("d")]
        public string Summary { get; set; }

        /// <summary>
        /// Authors of the package. May be <c>null</c>.
        /// </summary>
        [JsonProperty("a")]
        public IReadOnlyList<string> Authors { get; set; }

        /// <summary>
        /// Icon of the package. May be <c>null</c>.
        /// </summary>
        [JsonProperty("c")]
        public string IconUrl { get; set; }

        /// <summary>
        /// Project home page of the package. May be <c>null</c>.
        /// </summary>
        [JsonProperty("p")]
        public string ProjectUrl { get; set; }

        public override string ToString() => PackageId + "/" + Version;
    }
}
