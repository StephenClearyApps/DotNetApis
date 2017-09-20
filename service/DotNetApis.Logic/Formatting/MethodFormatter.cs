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
        /// Formats method parameters.
        /// </summary>
        /// <param name="member">The member whose parameters these are.</param>
        /// <param name="parameters">The parameters to format.</param>
        /// <param name="xmldoc">The XML documentation. May be <c>null</c>.</param>
        public IEnumerable<MethodParameter> Parameters(IMemberDefinition member, IEnumerable<ParameterDefinition> parameters, XContainer xmldoc) =>
            parameters.Select(p => Parameter(member, p, xmldoc));

        /// <summary>
        /// Formats a method parameter.
        /// </summary>
        /// <param name="member">The member whose parameter this is.</param>
        /// <param name="parameter">The parameter to format.</param>
        /// <param name="xmldoc">The XML documentation. May be <c>null</c>.</param>
        private MethodParameter Parameter(IMemberDefinition member, ParameterDefinition parameter, XContainer xmldoc)
        {
            var parameterType = parameter.ParameterType;
            var byRefParameterType = parameterType as ByReferenceType;
            if (byRefParameterType != null)
                parameterType = ((ByReferenceType)parameterType).ElementType;
            var decimalConstantAttribute = parameter.TryGetDecimalConstantAttribute();

            var modifiers = byRefParameterType != null && parameter.IsOut ? MethodParameterModifiers.Out :
                byRefParameterType != null && !parameter.IsOut ? MethodParameterModifiers.Ref :
                parameter.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ParamArrayAttribute") ? MethodParameterModifiers.Params :
                MethodParameterModifiers.None;
            var value = parameter.HasConstant ? _literalFormatter.Literal(parameter.ParameterType, parameter.Constant) :
                decimalConstantAttribute != null ? _literalFormatter.Literal(parameter.ParameterType, decimalConstantAttribute.GetDecimalValue()) :
                null;

            return new MethodParameter
            {
                Attributes = _attributeFormatter.Attributes(parameter).ToList(),
                Modifiers = modifiers,
                Type = _typeReferenceFormatter.TypeReference(parameterType, parameter.GetDynamicReplacement()),
                Name = _nameFormatter.EscapeIdentifier(parameter.Name),
                Value = value,
                XmldocNode = _xmldocFormatter.XmldocNodeForParameter(member, parameter, xmldoc),
            };
        }
    }
}
