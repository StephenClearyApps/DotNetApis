using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.Entities;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for an assembly within a NuGet package.
    /// </summary>
    public sealed class AssemblyJson
    {
        /// <summary>
        /// The full name of the assembly.
        /// </summary>
        [JsonProperty("n")]
        public string FullName { get; set; }

        /// <summary>
        /// The path (within the NuGet package) of the assembly.
        /// </summary>
        [JsonProperty("p")]
        public string Path { get; set; }

        /// <summary>
        /// The size of the assembly, in bytes.
        /// </summary>
        [JsonProperty("s")]
        public long FileLength { get; set; }

        /// <summary>
        /// Assembly-level attributes.
        /// </summary>
        [JsonProperty("b")]
        public IReadOnlyList<AttributeJson> Attributes { get; set; }

        /// <summary>
        /// Types defined by this assembly.
        /// </summary>
        [JsonProperty("t")]
        public IReadOnlyList<IEntity> Types { get; set; }

        public override string ToString() => Path;
    }
}
