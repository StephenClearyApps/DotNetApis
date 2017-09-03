using System.Collections.Generic;
using System.Linq;
using DotNetApis.Common;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public static partial class CecilExtensions
    {
        public static FriendlyName GetFriendlyName(this IMemberDefinition member)
        {
            var ns = member.DeclaringType != null ? member.DeclaringTypesInnerToOuter().Last().Namespace : ((TypeDefinition)member).Namespace;

            if (member is TypeDefinition type)
            {
                var simpleName = GetSimpleName(type);
                return new FriendlyName(simpleName, ns + "." + simpleName, ns + "." + simpleName);
            }

            var declaringType = string.Join(".", member.DeclaringType.GenericDeclaringTypesAndThis().Select(GetSimpleName));

            if (member is MethodDefinition method)
            {
                var methodName = method.Name.StripBacktickSuffix().Name;
                return CreateFromDeclaringType(GetSimpleName(methodName, method.GenericParameters), declaringType, ns);
            }

            return CreateFromDeclaringType(member.Name, declaringType, ns);
        }

        public static FriendlyName GetOverloadFriendlyName(this MethodDefinition method)
        {
            var ns = method.DeclaringTypesInnerToOuter().Last().Namespace;
            var simpleName = method.Name.StripBacktickSuffix().Name; // Note: no generic parameters
            var declaringType = string.Join(".", method.DeclaringType.GenericDeclaringTypesAndThis().Select(GetSimpleName));
            return CreateFromDeclaringType(simpleName, declaringType, ns);
        }

        private static FriendlyName CreateFromDeclaringType(string simpleName, string declaringType, string ns) =>
            new FriendlyName(simpleName, declaringType + "." + simpleName, ns + "." + declaringType + "." + simpleName);

        private static string GetSimpleName(TypeReference type) => string.Join(".", type.GenericDeclaringTypesAndThis().Select(GetSimpleName));

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
