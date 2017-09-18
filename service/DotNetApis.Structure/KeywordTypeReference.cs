using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A type reference associated with a C# keyword.
    /// </summary>
    public sealed class KeywordTypeReference : ITypeReference
    {
        public EntityReferenceKind Kind => EntityReferenceKind.Keyword;

        /// <summary>
        /// The name of the keyword, e.g., "int".
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The location of the actual type, e.g., the location of System.Int32.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }
    }
}
