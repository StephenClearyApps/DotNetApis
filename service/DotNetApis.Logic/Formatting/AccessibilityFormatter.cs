using System;
using DotNetApis.Cecil;
using DotNetApis.Structure.Entities;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    /// <summary>
    /// Formats entity accessibility flags.
    /// </summary>
    public sealed class AccessibilityFormatter
    {
        /// <summary>
        /// Formats the accessibility for a type definition.
        /// </summary>
        /// <param name="type">The type definition.</param>
        public EntityAccessibility TypeDefinitionAccessibility(TypeDefinition type)
        {
            if (type.IsPublic || type.IsNestedPublic)
                return EntityAccessibility.Public;
            if (type.IsNestedFamily)
                return EntityAccessibility.Protected;
            if (type.IsNestedFamilyOrAssembly)
                return EntityAccessibility.Protected;
            throw new InvalidOperationException($"Attempting to format an internal or private entity {type.FullName}");
        }

        /// <summary>
        /// Formats the accessibility modifier for a method.
        /// </summary>
        /// <param name="method">The method.</param>
        public EntityAccessibility MethodAccessibility(MethodDefinition method)
        {
            if (method.IsPublic)
                return EntityAccessibility.Public;
            if (method.IsFamilyOrAssembly)
                return EntityAccessibility.Protected;
            if (method.IsFamily)
                return EntityAccessibility.Protected;
            throw new InvalidOperationException($"Attempting to format an internal or private entity {method.FullName}");
        }

        /// <summary>
        /// Formats the accessibility modifier for a field.
        /// </summary>
        /// <param name="field">The field.</param>
        public EntityAccessibility FieldAccessibility(FieldDefinition field)
        {
            if (field.IsPublic)
                return EntityAccessibility.Public;
            if (field.IsFamilyOrAssembly)
                return EntityAccessibility.Protected;
            if (field.IsFamily)
                return EntityAccessibility.Protected;
            throw new InvalidOperationException($"Attempting to format an internal or private entity {field.FullName}");
        }

        /// <summary>
        /// Formats the accessibility modifier for a property.
        /// </summary>
        /// <param name="property">The property.</param>
        public (AccessibilityRestrictionLevel, EntityAccessibility) PropertyAccessibility(PropertyDefinition property)
        {
            if (Either(property.GetMethod, property.SetMethod, x => x.IsPublic))
                return (AccessibilityRestrictionLevel.Public, EntityAccessibility.Public);
            if (Either(property.GetMethod, property.SetMethod, x => x.IsFamilyOrAssembly))
                return (AccessibilityRestrictionLevel.ProtectedInternal, EntityAccessibility.Protected);
            if (Either(property.GetMethod, property.SetMethod, x => x.IsFamily))
                return (AccessibilityRestrictionLevel.Protected, EntityAccessibility.Protected);
            throw new InvalidOperationException($"Attempting to format an internal or private entity {property.FullName}");
        }

        /// <summary>
        /// Returns true if either method satisfies the predicate. At least one or the other of the methods must be non-<c>null</c>.
        /// </summary>
        /// <param name="first">The first method to test.</param>
        /// <param name="second">The second method to test.</param>
        /// <param name="predicate">The test to apply to these methods. This delegate is never passed a <c>null</c> argument.</param>
        private static bool Either(MethodDefinition first, MethodDefinition second, Func<MethodDefinition, bool> predicate)
        {
            if (first == null)
                return predicate(second);
            if (second == null)
                return predicate(first);
            return predicate(first) || predicate(second);
        }
    }
}
