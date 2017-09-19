using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.TypeReferences;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// An entity that can be defined by an interface, and thus can be implemented explicitly.
    /// </summary>
    public interface IHaveExplicitInterface
    {
        /// <summary>
        /// If this entity is an explicit interface definition, then this is the interface where the entity is defined.
        /// </summary>
        [JsonProperty("d")]
        ITypeReference ExplicitInterfaceDeclaringType { get; set; }
    }
}
