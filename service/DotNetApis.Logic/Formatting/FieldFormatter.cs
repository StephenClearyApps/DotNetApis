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
    public sealed class FieldFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly AccessibilityFormatter _accessibilityFormatter;
        private readonly TypeReferenceFormatter _typeReferenceFormatter;
        private readonly NameFormatter _nameFormatter;
        private readonly LiteralFormatter _literalFormatter;
        private readonly XmldocFormatter _xmldocFormatter;

        public FieldFormatter(AttributeFormatter attributeFormatter, AccessibilityFormatter accessibilityFormatter, TypeReferenceFormatter typeReferenceFormatter, NameFormatter nameFormatter,
            LiteralFormatter literalFormatter, XmldocFormatter xmldocFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _accessibilityFormatter = accessibilityFormatter;
            _typeReferenceFormatter = typeReferenceFormatter;
            _nameFormatter = nameFormatter;
            _literalFormatter = literalFormatter;
            _xmldocFormatter = xmldocFormatter;
        }

        /// <summary>
        /// Formats a field declaration, which may be a field or a constant.
        /// </summary>
        /// <param name="field">The field to format.</param>
        public FieldEntity Field(FieldDefinition field)
        {
            var result = new FieldEntity
            {
                DnaId = field.DnaId(),
                Attributes = _attributeFormatter.Attributes(field).ToList(),
                Accessibility = _accessibilityFormatter.FieldAccessibility(field),
                Modifiers = field.IsStatic ? EntityModifiers.Static : EntityModifiers.None,
                Name = _nameFormatter.EscapeIdentifier(field.Name),
                Type = _typeReferenceFormatter.TypeReference(field.FieldType, field.GetDynamicReplacement()),
                Xmldoc = _xmldocFormatter.Xmldoc(field),
            };

            var decimalConstantAttribute = field.TryGetDecimalConstantAttribute();
            if (field.HasConstant)
            {
                result.Modifiers |= EntityModifiers.Const;
                result.Value = _literalFormatter.Literal(field.FieldType, field.Constant);
            }
            else if (decimalConstantAttribute != null)
            {
                result.Modifiers |= EntityModifiers.Const;
                result.Value = _literalFormatter.Literal(field.FieldType, decimalConstantAttribute.GetDecimalValue());
            }

            return result;
        }
    }
}
