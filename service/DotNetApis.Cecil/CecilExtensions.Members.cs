using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public static partial class CecilExtensions
    {
        /// <summary>
        /// Returns all the declaring types for this member, starting with the member's immediate declaring type and working outward.
        /// </summary>
        public static IEnumerable<TypeReference> DeclaringTypesInnerToOuter(this IMemberDefinition @this)
        {
            while (@this.DeclaringType != null)
            {
                yield return @this.DeclaringType;
                @this = @this.DeclaringType;
            }
        }

        /// <summary>
        /// Gets the indexer parameters for this property. Returns an empty collection if this property is not an indexer property.
        /// </summary>
        public static IReadOnlyCollection<ParameterDefinition> GetIndexerParameters(this PropertyDefinition @this)
        {
            if (@this.GetMethod != null)
                return @this.GetMethod.Parameters.Reify();
            return @this.SetMethod.Parameters.Take(@this.SetMethod.Parameters.Count - 1).Reify();
        }

        /// <summary>
        /// Attempts to find the field backing an event definition. Returns <c>null</c> if there is no such field.
        /// </summary>
        public static FieldDefinition TryGetField(this EventDefinition @this) => @this.DeclaringType.Fields.FirstOrDefault(x => x.Name == @this.Name);
    }
}
