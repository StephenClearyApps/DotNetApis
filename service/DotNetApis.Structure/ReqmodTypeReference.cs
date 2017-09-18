using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a reqmod wrapper around an inner type reference.
    /// </summary>
    public sealed class ReqmodTypeReference : TypeReferenceBase
    {
        public override EntityReferenceKind Kind => EntityReferenceKind.Reqmod;

        /// <summary>
        /// The location of the reqmod type.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        /// <summary>
        /// The inner type reference that is wrapped by this reqmod type.
        /// </summary>
        [JsonProperty("t")]
        public TypeReferenceBase ElementType { get; set; }
    }
}
