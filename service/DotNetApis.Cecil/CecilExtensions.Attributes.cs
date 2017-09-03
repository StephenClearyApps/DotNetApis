using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public static partial class CecilExtensions
    {
        /// <summary>
        /// Retrieves the decimal value specified by a <c>[DecimalConstantAttribute]</c>.
        /// </summary>
        public static decimal GetDecimalValue(this CustomAttribute @this)
        {
            var arg1 = (byte)@this.ConstructorArguments[0].Value;
            var arg2 = (byte)@this.ConstructorArguments[1].Value;
            // ReSharper disable once MergeCastWithTypeCheck
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
    }
}
