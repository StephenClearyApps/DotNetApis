using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Structure.Entities;
using DotNetApis.Structure.Literals;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    public sealed class MethodFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly TypeReferenceFormatter _typeReferenceFormatter;
        private readonly NameFormatter _nameFormatter;
        private readonly LiteralFormatter _literalFormatter;
        private readonly XmldocFormatter _xmldocFormatter;

        public MethodFormatter(AttributeFormatter attributeFormatter, TypeReferenceFormatter typeReferenceFormatter, NameFormatter nameFormatter, LiteralFormatter literalFormatter,
            XmldocFormatter xmldocFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _typeReferenceFormatter = typeReferenceFormatter;
            _nameFormatter = nameFormatter;
            _literalFormatter = literalFormatter;
            _xmldocFormatter = xmldocFormatter;
        }

        /// <summary>
        /// Formats a method parameter.
        /// </summary>
        /// <param name="member">The member whose parameter this is.</param>
        /// <param name="parameter">The parameter to format.</param>
        /// <param name="xmldoc">The XML documentation. May be <c>null</c>.</param>
        public MethodParameter Parameter(IMemberDefinition member, ParameterDefinition parameter, XContainer xmldoc)
        {
            var parameterType = parameter.ParameterType;
            var byRefParameterType = parameterType as ByReferenceType;
            if (byRefParameterType != null)
                parameterType = ((ByReferenceType)parameterType).ElementType;
            var decimalConstantAttribute = parameter.TryGetDecimalConstantAttribute();

            var attributes = _attributeFormatter.Attributes(parameter);
            var isOut = byRefParameterType != null && parameter.IsOut;
            var isRef = byRefParameterType != null && !parameter.IsOut;
            var isParams = parameter.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ParamArrayAttribute");
            var type = _typeReferenceFormatter.TypeReference(parameterType, parameter.GetDynamicReplacement());
            var name = _nameFormatter.EscapeIdentifier(parameter.Name);
            ILiteral value = null;

            if (parameter.HasConstant)
                value = _literalFormatter.Literal(parameter.ParameterType, parameter.Constant);
            else if (decimalConstantAttribute != null)
                value = _literalFormatter.Literal(parameter.ParameterType, decimalConstantAttribute.GetDecimalValue());
            return new MethodParameter
            {
                Attributes = attributes,
                Modifiers = isOut ? MethodParameterModifiers.Out :
                        isRef ? MethodParameterModifiers.Ref :
                        isParams ? MethodParameterModifiers.Params :
                        MethodParameterModifiers.None,
                Type = type,
                Name = name,
                Value = value,
                Xmldoc = _xmldocFormatter.XmldocNode(member, parameter, xmldoc),
            };
        }
    }
}
