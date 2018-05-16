using System.Collections.Generic;
using System.Linq;
using DotNetApis.Common;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public static partial class CecilExtensions
    {
        /// <summary>
        /// Gets a friendly name for the member.
        /// </summary>
        public static FriendlyName MemberFriendlyName(this IMemberDefinition member)
        {
            var ns = member.DeclaringType != null ? member.DeclaringTypesInnerToOuter().Last().Namespace : ((TypeDefinition)member).Namespace;

            if (member is TypeDefinition type)
            {
                var simpleName = GetSimpleName(type);
                return new FriendlyName(simpleName, ns.DotAppend(simpleName), ns.DotAppend(simpleName));
            }

            var declaringType = string.Join(".", member.DeclaringType.GenericDeclaringTypesAndThis().Select(GetSimpleName));

            if (member is MethodDefinition method)
            {
                var simpleName = GetSimpleName(method.SimpleMethodName(), method.GenericParameters);
                return CreateFromDeclaringType(simpleName, declaringType, ns);
            }

            return CreateFromDeclaringType(member.Name, declaringType, ns);
        }

        /// <summary>
        /// Gets a method name, using C# conventions instead of <c>.ctor</c>, <c>..ctor</c>, or <c>Finalize</c>. The returned name does not have a backtick suffix, nor does it have any generic parameters.
        /// </summary>
        public static string SimpleMethodName(this MethodDefinition method)
        {
            var methodName = method.Name.StripBacktickSuffix().Name;
            if (methodName == ".ctor" || methodName == ".cctor")
                methodName = method.DeclaringType.Name.StripBacktickSuffix().Name;
            else if (methodName == "Finalize")
                methodName = "~" + method.DeclaringType.Name.StripBacktickSuffix().Name;
            return methodName;
        }

        /// <summary>
        /// Get a friendly name for the method overload group.
        /// </summary>
        public static FriendlyName OverloadFriendlyName(this MethodDefinition method)
        {
            var ns = method.DeclaringTypesInnerToOuter().Last().Namespace;
            var simpleName = method.SimpleMethodName();
            var declaringType = string.Join(".", method.DeclaringType.GenericDeclaringTypesAndThis().Select(GetSimpleName));
            return CreateFromDeclaringType(simpleName, declaringType, ns); // Note: no generic parameters for overloads
        }

        private static FriendlyName CreateFromDeclaringType(string simpleName, string declaringType, string ns) =>
            new FriendlyName(simpleName, declaringType + "." + simpleName, ns.DotAppend(declaringType + "." + simpleName));

        private static string GetSimpleName(TypeReference type) => string.Join(".", type.GenericDeclaringTypesAndThis().Select(GetSimpleName));

        /// <summary>
        /// Formats a name with its generic parameters, e.g., "Task&lt;TResult&gt;". If there are no generic parameters, then there are no angle brackets in the output, e.g., "ArrayList".
        /// </summary>
        /// <param name="type">The type.</param>
        private static string GetSimpleName(GenericTypeReference type) => GetSimpleName(type.Name, type.GenericParameters);

        /// <summary>
        /// Formats a name with its generic parameters, e.g., "Task&lt;TResult&gt;". If there are no generic parameters, then there are no angle brackets in the output, e.g., "ArrayList".
        /// </summary>
        /// <param name="nameWithoutBacktickSuffix">The name, without the backtick suffix.</param>
        /// <param name="genericParameters">The generic parameters, if any.</param>
        private static string GetSimpleName(string nameWithoutBacktickSuffix, IEnumerable<GenericParameter> genericParameters)
        {
            // This approach assumes that cref attributes can only refer to generic types (e.g., List<T>), not concrete generic types (e.g., List<string>).
            var parameters = genericParameters.Reify();
            if (!parameters.Any())
                return nameWithoutBacktickSuffix;
            return nameWithoutBacktickSuffix + "<" + string.Join(",", parameters.Select(x => x.Name)) + ">";
        }
    }
}
