using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
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

    public static partial class CecilExtensions
    {
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
    }
}
