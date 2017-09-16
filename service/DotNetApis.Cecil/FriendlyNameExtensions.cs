using DotNetApis.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public static class FriendlyNameExtensions
    {
        public static FriendlyName CreateFriendlyName(this IMemberDefinition member)
        {
            var ns = member.DeclaringType != null ? member.DeclaringTypesInnerToOuter().Last().Namespace : ((TypeDefinition)member).Namespace;

            if (member is TypeDefinition type)
            {
                var simpleName = GetSimpleName(type);
                return CreateFromType(simpleName, ns);
            }

            var declaringType = string.Join(".", member.DeclaringType.GenericDeclaringTypesAndThis().Select(GetSimpleName));

            if (member is MethodDefinition method)
            {
                var methodName = method.Name.StripBacktickSuffix().Name;
                return CreateFromDeclaringType(GetSimpleName(methodName, method.GenericParameters), declaringType, ns);
            }

            return CreateFromDeclaringType(member.Name, declaringType, ns);
        }

        public static FriendlyName CreateOverloadFriendlyName(this MethodDefinition method)
        {
            var ns = method.DeclaringTypesInnerToOuter().Last().Namespace;
            var simpleName = method.Name.StripBacktickSuffix().Name; // Note: no generic parameters
            var declaringType = string.Join(".", method.DeclaringType.GenericDeclaringTypesAndThis().Select(GetSimpleName));
            return CreateFromDeclaringType(simpleName, declaringType, ns);
        }

        private static FriendlyName CreateFromDeclaringType(string simpleName, string declaringType, string ns) =>
            new FriendlyName(simpleName, declaringType + "." + simpleName, ns + "." + declaringType + "." + simpleName);

        private static FriendlyName CreateFromType(string simpleName, string ns) => new FriendlyName(simpleName, ns + "." + simpleName, ns + "." + simpleName);

        private static string GetSimpleName(TypeReference type) => string.Join(".", type.GenericDeclaringTypesAndThis().Select(GetSimpleName));

        private static string GetSimpleName(GenericTypeReference type) => GetSimpleName(type.Name, type.GenericParameters);

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
