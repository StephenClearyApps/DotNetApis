using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// A location in a package that is a dependency for the current package.
    /// </summary>
    public sealed class DependencyLocation
    {
        /// <summary>
        /// The id of the dependency package containing this location. This should use regular casing, not lowercased.
        /// </summary>
        [JsonProperty("p")]
        public string PackageId { get; set; }

        /// <summary>
        /// The version of the dependency package used during documentation generation. Canonical format.
        /// </summary>
        [JsonProperty("v")]
        public string PackageVersion { get; set; }

        /// <summary>
        /// The DNA ID of the location.
        /// </summary>
        [JsonProperty("i")]
        public string DnaId { get; set; }
    }
}
