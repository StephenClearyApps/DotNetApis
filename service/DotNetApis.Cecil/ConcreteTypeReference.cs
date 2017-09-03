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
    /// A type combined with its generic arguments, if any.
    /// </summary>
    public struct ConcreteTypeReference
    {
        /// <summary>
        /// The name of the type. If this is a generic type, then this name has the backtick suffix stripped.
        /// </summary>
        public string Name;

        /// <summary>
        /// The type reference.
        /// </summary>
        public TypeReference TypeReference;

        /// <summary>
        /// The generic arguments for this type. If there are none, then this is an empty collection.
        /// </summary>
        public IReadOnlyList<TypeReference> GenericArguments;
    }

    public static partial class CecilExtensions
    {
        /// <summary>
        /// Enumerates this type reference and all of its declaring type, walking "down" from the top-level type and ending at this type reference, applying the generic arguments appropriately among the declaring types and this type.
        /// </summary>
        public static IEnumerable<ConcreteTypeReference> ConcreteDeclaringTypesAndThis(this GenericInstanceType @this)
        {
            var argumentIndex = 0;
            var arguments = @this.GenericArguments.ToArray();
            foreach (var type in @this.DeclaringTypesAndThis())
            {
                var name = type.Name.StripBacktickSuffix();
                yield return new ConcreteTypeReference
                {
                    Name = name.Name,
                    TypeReference = type,
                    GenericArguments = new ArraySegment<TypeReference>(arguments, argumentIndex, name.Value),
                };
                argumentIndex += name.Value;
            }
        }
    }
}
