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

        // TODO: Move all these "XContainer xmldoc" parameters to an implicit context, along with AssemblyCollection and TargetFramework.

        /// <summary>
        /// Formats a field declaration, which may be a field or a constant.
        /// </summary>
        /// <param name="field">The field to format.</param>
        /// <param name="xmldoc">The XML documentation. May be <c>null</c>.</param>
        public FieldEntity Field(FieldDefinition field, XContainer xmldoc)
        {
            var decimalConstantAttribute = field.TryGetDecimalConstantAttribute();

            var modifiers = field.IsStatic ? EntityModifiers.Static : EntityModifiers.None;
            ILiteral value = null;

            if (field.HasConstant)
            {
                modifiers |= EntityModifiers.Const;
                value = _literalFormatter.Literal(field.FieldType, field.Constant);
            }
            else if (decimalConstantAttribute != null)
            {
                modifiers |= EntityModifiers.Const;
                value = _literalFormatter.Literal(field.FieldType, decimalConstantAttribute.GetDecimalValue());
            }

            return new FieldEntity
            {
                DnaId = field.DnaId(),
                Attributes = _attributeFormatter.Attributes(field).ToList(),
                Accessibility = _accessibilityFormatter.FieldAccessibility(field),
                Name = _nameFormatter.EscapeIdentifier(field.Name),
                Modifiers = modifiers,
                Type = _typeReferenceFormatter.TypeReference(field.FieldType, field.GetDynamicReplacement()),
                Value = value,
                Xmldoc = _xmldocFormatter.Xmldoc(field, xmldoc),
            };
        }
    }
}
