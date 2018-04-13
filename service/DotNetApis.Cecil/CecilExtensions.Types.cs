using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public static partial class CecilExtensions
    {
        /// <summary>
        /// Enumerates this type reference and all of its declaring type, walking "up" from most-nested to top-level.
        /// </summary>
        private static IEnumerable<TypeReference> ThisAndDeclaringTypes(TypeReference @this)
        {
            yield return @this;
            while (@this.DeclaringType != null)
            {
                @this = @this.DeclaringType;
                yield return @this;
            }
        }

        /// <summary>
        /// Enumerates this type reference and all of its declaring type, walking "down" from the top-level type and ending at this type reference.
        /// </summary>
        public static IEnumerable<TypeReference> DeclaringTypesAndThis(this TypeReference @this) => ThisAndDeclaringTypes(@this).Reverse();

        /// <summary>
        /// Returns the base type (if other than Object, ValueType, MulticastDelegate, or Enum) and the interfaces for this type. Does not return implicitly-implemented interfaces.
        /// </summary>
        public static IEnumerable<TypeReference> BaseTypeAndInterfaces(this TypeDefinition type)
        {
            var baseType = type.BaseType;
            if (baseType != null && baseType.FullName != "System.Object" && baseType.FullName != "System.ValueType" && baseType.FullName != "System.Enum" && baseType.FullName != "System.MulticastDelegate")
                yield return baseType;
            foreach (var i in type.Interfaces)
                yield return i;
        }

        /// <summary>
        /// Whether an enum type definition has the <c>[Flags]</c> attribute.
        /// </summary>
        public static bool EnumHasFlagsAttribute(this TypeDefinition @this) => @this.CustomAttributes.Any(x => x.AttributeType.Namespace == "System" && x.AttributeType.Name == "FlagsAttribute");
    }
}
