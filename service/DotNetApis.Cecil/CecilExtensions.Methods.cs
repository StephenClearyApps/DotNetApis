using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Whether this method definition is an override of another method.
        /// </summary>
        public static bool IsOverride(this MethodDefinition @this) => @this.GetBaseMethod() != @this;

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
    }
}
