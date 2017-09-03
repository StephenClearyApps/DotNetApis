using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    /// <summary>
    /// A member reference combined with its generic parameters, if any.
    /// </summary>
    public struct GenericTypeReference
    {
        /// <summary>
        /// The name of the type. If this is a generic type, then this name has the backtick suffix stripped.
        /// </summary>
        public string Name;

        /// <summary>
        /// The type reference.
        /// </summary>
        public TypeReference Reference;

        /// <summary>
        /// The generic parameters for this type. If there are none, then this is an empty collection.
        /// </summary>
        public IReadOnlyList<GenericParameter> GenericParameters;
    }

    public static partial class CecilExtensions
    {
        /// <summary>
        /// Enumerates this type reference and all of its declaring type, walking "down" from the top-level type and ending at this type reference, applying the generic parameters appropriately among the declaring types and this type.
        /// </summary>
        public static IEnumerable<GenericTypeReference> GenericDeclaringTypesAndThis(this TypeReference @this)
        {

            var argumentIndex = 0;
            var arguments = @this.GenericParameters.ToArray();
            foreach (var declaringType in @this.DeclaringTypesAndThis())
            {
                var name = declaringType.Name.StripBacktickSuffix();
                yield return new GenericTypeReference
                {
                    Name = name.Name,
                    Reference = declaringType,
                    GenericParameters = new ArraySegment<GenericParameter>(arguments, argumentIndex, name.Value),
                };
                argumentIndex += name.Value;
            }
        }
    }
}
