using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.TypeReferences;
using Newtonsoft.Json;

namespace DotNetApis.Structure.GenericConstraints
{
    /// <summary>
    /// A type generic constraint.
    /// </summary>
    public sealed class TypeGenericConstraint : IGenericConstraint
    {
        public GenericConstraintKind Kind => GenericConstraintKind.Type;

        /// <summary>
        /// The type that the generic parameter must be assignable to.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference Type { get; set; }
    }
}
