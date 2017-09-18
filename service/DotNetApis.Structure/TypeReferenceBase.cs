using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a type reference.
    /// </summary>
    public interface ITypeReference
    {
        /// <summary>
        /// The kind of type reference this is.
        /// </summary>
        [JsonProperty("k")]
        EntityReferenceKind Kind { get; }
    }
}
