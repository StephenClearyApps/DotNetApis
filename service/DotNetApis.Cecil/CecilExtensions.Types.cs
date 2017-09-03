using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace DotNetApis.Cecil
{
    public static partial class CecilExtensions
    {
        /// <summary>
        /// Whether a type is a delegate.
        /// </summary>
        public static bool IsDelegate(this TypeDefinition typeDefinition) => typeDefinition.BaseType != null && typeDefinition.BaseType.FullName == "System.MulticastDelegate";

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

        /// <summary>
        /// Applies the generic arguments appropriately among the declaring types and this type.
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
        /// Applies the generic arguments appropriately among the declaring types and this type.
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

        /// <summary>
        /// Enumerates this type reference and all of its declaring type, walking "down" from the top-level type and ending at this type reference.
        /// </summary>
        public static IEnumerable<TypeReference> DeclaringTypesAndThis(this TypeReference @this) => ThisAndDeclaringTypes(@this).Reverse();

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
        /// The type of a generic constraint.
        /// </summary>
        public enum GenericConstraintType
        {
            /// <summary>
            /// A reference constraint.
            /// </summary>
            ReferenceType,

            /// <summary>
            /// A value type constraint.
            /// </summary>
            NonNullableValueType,

            /// <summary>
            /// A default constructor constraint.
            /// </summary>
            DefaultConstructor,

            /// <summary>
            /// A type constraint.
            /// </summary>
            Type,
        }

        /// <summary>
        /// A constraint on a generic parameter.
        /// </summary>
        public struct GenericConstraint
        {
            /// <summary>
            /// The type of the generic constraint.
            /// </summary>
            public GenericConstraintType Type { get; set; }

            /// <summary>
            /// If <see cref="Type"/> is <see cref="GenericConstraintType.Type"/>, then this member is the type to which the generic parameter is constrained. Otherwise, this is <c>null</c>.
            /// </summary>
            public TypeReference TypeReference { get; set; }
        }

        /// <summary>
        /// Enumerates the generic constraints for a parameter. Reference/value type constrains are first, followed by all type constraints, and finally the default constructor constraint (if present).
        /// </summary>
        public static IEnumerable<GenericConstraint> GenericConstraints(this GenericParameter @this)
        {
            if (@this.HasReferenceTypeConstraint)
                yield return new GenericConstraint { Type = GenericConstraintType.ReferenceType };
            if (@this.HasNotNullableValueTypeConstraint)
                yield return new GenericConstraint { Type = GenericConstraintType.NonNullableValueType };
            foreach (var typeConstraint in @this.Constraints.Where(x => x.Namespace != "System" || x.Name != "ValueType"))
                yield return new GenericConstraint { Type = GenericConstraintType.Type, TypeReference = typeConstraint };
            if (@this.HasDefaultConstructorConstraint && !@this.HasNotNullableValueTypeConstraint)
                yield return new GenericConstraint { Type = GenericConstraintType.DefaultConstructor };
        }

        /// <summary>
        /// Whether an enum type definition has the <c>[Flags]</c> attribute.
        /// </summary>
        public static bool EnumHasFlagsAttribute(this TypeDefinition @this)
        {
            return @this.CustomAttributes.Any(x => x.AttributeType.Namespace == "System" && x.AttributeType.Name == "FlagsAttribute");
        }

        /// <summary>
        /// An argument for a custom attribute. This may be a positional or named argument.
        /// </summary>
        public struct AttributeConstructorArgument
        {
            /// <summary>
            /// The name of the attribute's property set by this argument. This is <c>null</c> if this argument is a positional argument.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The type of the value of this argument.
            /// </summary>
            public TypeReference Type { get; set; }

            /// <summary>
            /// The value of this argument.
            /// </summary>
            public object Value { get; set; }
        }

        /// <summary>
        /// Enumerates the arguments for a custom attribute. Positional arguments are enumerated first, followed by named arguments.
        /// </summary>
        public static IEnumerable<AttributeConstructorArgument> AttributeConstructorArguments(this CustomAttribute @this)
        {
            foreach (var argument in @this.ConstructorArguments)
                yield return new AttributeConstructorArgument { Type = argument.Type, Value = argument.Value };
            foreach (var argument in @this.Properties)
                yield return new AttributeConstructorArgument { Name = argument.Name, Type = argument.Argument.Type, Value = argument.Argument.Value };
        }


        /// <summary>
        /// A numeric evaluation of how restrictive accessors are, with the lowest value for the least restrictive (i.e., <c>public</c>), and the highest value for the most restrictive (i.e., <c>private</c>).
        /// </summary>
        public enum AccessibilityRestrictionLevel
        {
            Public = 0,
            ProtectedInternal = 1,
            Protected = 2,
            Internal = 2,
            Private = 4,
        }

        /// <summary>
        /// Gets the accessibility restriction level of a method.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static AccessibilityRestrictionLevel GetAccessibilityRestrictionLevel(this MethodDefinition @this)
        {
            if (@this.IsPublic)
                return AccessibilityRestrictionLevel.Public;
            if (@this.IsFamilyOrAssembly)
                return AccessibilityRestrictionLevel.ProtectedInternal;
            if (@this.IsFamily)
                return AccessibilityRestrictionLevel.Protected;
            if (@this.IsAssembly)
                return AccessibilityRestrictionLevel.Internal;
            return AccessibilityRestrictionLevel.Private;
        }

        /// <summary>
        /// Whether this method definition is an override of another method.
        /// </summary>
        public static bool IsOverride(this MethodDefinition @this) => @this.GetBaseMethod() != @this;

        /// <summary>
        /// Retrieves the decimal value specified by a <c>[DecimalConstantAttribute]</c>.
        /// </summary>
        public static decimal GetDecimalValue(this CustomAttribute @this)
        {
            var arg1 = (byte)@this.ConstructorArguments[0].Value;
            var arg2 = (byte)@this.ConstructorArguments[1].Value;
            if (@this.ConstructorArguments[2].Value is int)
            {
                var arg3 = (int)@this.ConstructorArguments[2].Value;
                var arg4 = (int)@this.ConstructorArguments[3].Value;
                var arg5 = (int)@this.ConstructorArguments[4].Value;
                return new System.Runtime.CompilerServices.DecimalConstantAttribute(arg1, arg2, arg3, arg4, arg5).Value;
            }
            else
            {
                var arg3 = (uint)@this.ConstructorArguments[2].Value;
                var arg4 = (uint)@this.ConstructorArguments[3].Value;
                var arg5 = (uint)@this.ConstructorArguments[4].Value;
                return new System.Runtime.CompilerServices.DecimalConstantAttribute(arg1, arg2, arg3, arg4, arg5).Value;
            }
        }

        /// <summary>
        /// Returns the <c>[DecimalConstantAttribute]</c> for this target, or <c>null</c> if there is no <c>[DecimalConstantAttribute]</c>.
        /// </summary>
        public static CustomAttribute TryGetDecimalConstantAttribute(this ICustomAttributeProvider @this) =>
            @this.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.DecimalConstantAttribute");

        /// <summary>
        /// Whether this method is an extension method.
        /// </summary>
        public static bool IsExtensionMethod(this MethodDefinition @this) => @this.CustomAttributes.Any(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.ExtensionAttribute");

        /// <summary>
        /// Returns the original interface method that this method explicitly implements. If this method is not an explicit interface implementation, returns <c>null</c>.
        /// </summary>
        public static MethodReference GetExplicitlyImplementedInterfaceMethod(this MethodDefinition @this)
        {
            if (!@this.IsVirtual || !@this.IsFinal || !@this.IsNewSlot || !@this.Name.Contains("."))
                return null;
            return @this.Overrides[0];
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

        /// <summary>
        /// Used to determine which types should be treated as <c>dynamic</c>.
        /// </summary>
        public sealed class DynamicReplacement
        {
            private int _nextIndex;
            private readonly bool[] _values;

            private DynamicReplacement()
            {
                _values = null;
            }

            private DynamicReplacement(int nextIndex)
            {
                _nextIndex = nextIndex;
            }

            public DynamicReplacement(bool[] values)
            {
                _values = values;
            }

            private static readonly DynamicReplacement NoDynamicInstance = new DynamicReplacement();
            private static readonly DynamicReplacement SingleDynamicInstance = new DynamicReplacement(-1);

            /// <summary>
            /// Gets an instance that never replaces types with <c>dynamic</c>.
            /// </summary>
            public static DynamicReplacement NoDynamic { get { return NoDynamicInstance; } }

            /// <summary>
            /// Gets an instance that always replaces types with <c>dynamic</c> (in practice, this results in a single, top-level <c>dynamic</c>).
            /// </summary>
            public static DynamicReplacement SingleDynamic { get { return SingleDynamicInstance; } }

            /// <summary>
            /// Whether the current type should be replaced with <c>dynamic</c>. This method must be called in a prefix traversal of a type's construction.
            /// </summary>
            /// <returns></returns>
            public bool CheckDynamicAndIncrement()
            {
                if (_nextIndex < 0)
                    return true;
                if (_values == null || _nextIndex >= _values.Length)
                    return false;
                var result = _values[_nextIndex];
                ++_nextIndex;
                return result;
            }
        }

        /// <summary>
        /// Gets the <see cref="DynamicReplacement"/> for this declaration.
        /// </summary>
        public static DynamicReplacement GetDynamicReplacement(this ICustomAttributeProvider @this)
        {
            var attribute = @this.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.DynamicAttribute");
            if (attribute == null)
                return DynamicReplacement.NoDynamic;
            if (attribute.ConstructorArguments.Count == 0)
                return DynamicReplacement.SingleDynamic;
            return new DynamicReplacement(((CustomAttributeArgument[])attribute.ConstructorArguments[0].Value).Select(x => (bool)x.Value).ToArray());
        }
    }
}
