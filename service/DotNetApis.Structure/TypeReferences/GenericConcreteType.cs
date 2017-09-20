using System.Collections.Generic;
using DotNetApis.Structure.Locations;
using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for a generic type combined with its generic type arguments.
    /// </summary>
    public sealed class GenericConcreteType
    {
        /// <summary>
        /// The name of the generic type, without a backtick suffix.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The location of the generic type.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        /// <summary>
        /// The generic type arguments.
        /// </summary>
        [JsonProperty("a")]
        public IReadOnlyList<ITypeReference> GenericArguments { get; set; }
    }
}
