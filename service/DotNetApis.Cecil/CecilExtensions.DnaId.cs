using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// Returns the DotNetApi identifier for this member.
        /// </summary>
        public static string MemberDnaId(this IMemberDefinition member)
        {
            if (member is TypeReference type)
                return type.DnaId();
            if (member is EventReference evt)
                return evt.DnaId();
            if (member is FieldReference field)
                return field.DnaId();
            if (member is PropertyReference property)
                return property.DnaId();
            if (member is MethodReference method)
                return method.DnaId();
            throw new InvalidOperationException("Unknown member definition type " + member.GetType().Name);
        }

        /// <summary>
        /// Returns the DotNetApi identifier for this type.
        /// </summary>
        public static string DnaId(this TypeReference type)
        {
            if (type.DeclaringType != null)
                return type.DeclaringType.DnaId() + "/" + type.Name.DnaEncode();
            var ns = type.Namespace.DnaEncode();
            if (ns != "")
                ns += ".";
            return ns + type.Name.DnaEncode();
        }

        /// <summary>
        /// Returns the DotNetApi identifier for this event.
        /// </summary>
        public static string DnaId(this EventReference evt) => evt.DeclaringType.DnaId() + "/" + evt.Name.DnaEncode();

        /// <summary>
        /// Returns the DotNetApi identifier for this field.
        /// </summary>
        public static string DnaId(this FieldReference field) => field.DeclaringType.DnaId() + "/" + field.Name.DnaEncode();

        /// <summary>
        /// Returns the DotNetApi identifier for this property.
        /// </summary>
        public static string DnaId(this PropertyReference property)
        {
            var result = property.DeclaringType.DnaId() + "/" + property.Name.DnaEncode();
            if (property.Parameters.Count == 0)
                return result;
            return result + "(" + string.Join(",", property.Parameters.Select(DnaParameterName)) + ")";
        }

        /// <summary>
        /// Returns the DotNetApi identifier for this method.
        /// </summary>
        public static string DnaId(this MethodReference method)
        {
            var result = method.DeclaringType.DnaId() + "/" + method.Name.DnaEncode();
            if (method.HasGenericParameters)
                result += "''" + method.GenericParameters.Count.ToString(CultureInfo.InvariantCulture);
            result += "(" + string.Join(",", method.Parameters.Select(DnaParameterName)) + ")";
            if (method.Name == "op_Implicit" || method.Name == "op_Explicit")
                result += "~" + method.ReturnType.DnaParameterName();
            return result;
        }

        /// <summary>
        /// Returns the DotNetApi identifier for this method's overload group.
        /// </summary>
        public static string OverloadDnaId(this MethodReference method) => method.DeclaringType.DnaId() + "/" + method.Name.DnaEncode();

        /// <summary>
        /// Returns the DotNetApi identifier for this parameter.
        /// </summary>
        private static string DnaParameterName(this ParameterDefinition parameter) => parameter.ParameterType.DnaParameterName();

        /// <summary>
        /// Returns the DotNetApi name for this type reference when used as a parameter.
        /// </summary>
        private static string DnaParameterName(this TypeReference type)
        {
            if (type is GenericParameter genericParameter)
            {
                if (genericParameter.DeclaringMethod == null)
                    return "'" + genericParameter.Position.ToString(CultureInfo.InvariantCulture);
                return "''" + genericParameter.Position.ToString(CultureInfo.InvariantCulture);
            }

            if (type is PointerType pointerType)
                return pointerType.ElementType.DnaParameterName() + "~";

            if (type is ArrayType arrayType)
            {
                // Special encoding for simple arrays.
                if (arrayType.Dimensions.Count == 1 && !arrayType.Dimensions[0].UpperBound.HasValue &&
                    (!arrayType.Dimensions[0].LowerBound.HasValue || arrayType.Dimensions[0].LowerBound.Value == 0))
                    return arrayType.ElementType.DnaParameterName() + "$";
                return arrayType.ElementType.DnaParameterName() + "@5B" + string.Join(",", arrayType.Dimensions.Select(DnaParameterName)) + "@5D";
            }

            if (type is ByReferenceType byrefType)
                return byrefType.ElementType.DnaParameterName() + "-";

            if (type is OptionalModifierType optmodType)
                return optmodType.ElementType.DnaParameterName() + "!" + optmodType.ModifierType.DnaParameterName();

            if (type is RequiredModifierType reqmodType)
                return reqmodType.ElementType.DnaParameterName() + "=" + reqmodType.ModifierType.DnaParameterName();

            if (type is GenericInstanceType genericInstance)
                return string.Join("/", genericInstance.ConcreteDeclaringTypesAndThis().Select(DnaParameterName));

            // It's a fully-qualified reference to a type.
            return type.DnaId();
        }

        private static string DnaParameterName(this ConcreteTypeReference genericTypeReference)
        {
            var type = genericTypeReference.TypeReference;
            var result = string.Empty;
            if (!type.IsNested)
                result = type.Namespace.DnaEncode() + ".";
            result += genericTypeReference.Name.DnaEncode();
            if (genericTypeReference.GenericArguments.Count != 0)
                result += "(" + string.Join(",", genericTypeReference.GenericArguments.Select(DnaParameterName)) + ")";
            return result;
        }

        /// <summary>
        /// Returns the DotNetApi name for this array dimension.
        /// </summary>
        private static string DnaParameterName(this ArrayDimension arrayDimension)
        {
            if (!arrayDimension.LowerBound.HasValue && !arrayDimension.UpperBound.HasValue)
                return string.Empty;
            var result = new StringBuilder();
            if (arrayDimension.LowerBound.HasValue)
                result.Append(arrayDimension.LowerBound.Value.ToString(CultureInfo.InvariantCulture));
            result.Append(";");
            if (arrayDimension.UpperBound.HasValue)
                result.Append(arrayDimension.UpperBound.Value.ToString(CultureInfo.InvariantCulture));
            return result.ToString();
        }

        private static readonly char[] DotNetEncodingChars = {'\'', '`', '<', '(', '>', ')'};

        private static char ReplacementChar(char ch)
        {
            switch (ch)
            {
                case '\'': return '`';
                case '`': return '\'';
                case '<': return '(';
                case '(': return '<';
                case '>': return ')';
                case ')': return '>';
                default: return ch;
            }
        }

        private static readonly char[] DnaNameAlphabet =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '_', '.', '\'', '(', ')'
        };

        private static string DnaEncode(this string name)
        {
            // First, swap the following pairs: (',`), (<,(), (>, )).
            var result = name;
            if (name.IndexOfAny(DotNetEncodingChars) != -1)
            {
                var sb = new StringBuilder(name.Length);
                foreach (var ch in name)
                    sb.Append(ReplacementChar(ch));
                result = sb.ToString();
            }

            // Then, @-encode it.
            return result.AtEncode(DnaNameAlphabet);
        }
    }
}