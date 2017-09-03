using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
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

    public static partial class CecilExtensions
    {
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
    }
}
