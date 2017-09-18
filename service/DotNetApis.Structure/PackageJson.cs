using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured representation of a primary package. This is the "entry point" for structured representations.
    /// </summary>
    public sealed class PackageJson
    {
        /// <summary>
        /// The package id.
        /// </summary>
        [JsonProperty("i")]
        public string PackageId { get; set; }

        /// <summary>
        /// The package version.
        /// </summary>
        [JsonProperty("v")]
        public string Version { get; set; }

        /// <summary>
        /// The target that this JSON was generated for.
        /// </summary>
        [JsonProperty("t")]
        public string Target { get; set; }

        /// <summary>
        /// The description of the package.
        /// </summary>
        [JsonProperty("d")]
        public string Description { get; set; }

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

        /// <summary>
        /// All of the supported targets of this package.
        /// </summary>
        [JsonProperty("f")]
        public IReadOnlyList<string> SupportedTargets { get; set; }

        /// <summary>
        /// The dependencies for this package (both direct and indirect).
        /// </summary>
        [JsonProperty("e")]
        public IReadOnlyList<PackageDependencyJson> Dependencies { get; set; }

        /// <summary>
        /// The publication time of this package.
        /// </summary>
        [JsonProperty("b")]
        public DateTimeOffset Published { get; set; }

        /// <summary>
        /// Whether this package is a release version.
        /// </summary>
        [JsonProperty("r")]
        public bool IsReleaseVersion { get; set; }

        /// <summary>
        /// Structured documentation for each of the assemblies in this package.
        /// </summary>
        [JsonProperty("l")]
        public IReadOnlyList<AssemblyJson> Assemblies { get; set; }
    }
}
