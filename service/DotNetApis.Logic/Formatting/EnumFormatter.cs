using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Structure.Entities;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace DotNetApis.Logic.Formatting
{
    public sealed class EnumFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly AccessibilityFormatter _accessibilityFormatter;
        private readonly NameFormatter _nameFormatter;
        private readonly XmldocFormatter _xmldocFormatter;

        public EnumFormatter(AttributeFormatter attributeFormatter, AccessibilityFormatter accessibilityFormatter, NameFormatter nameFormatter, XmldocFormatter xmldocFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _accessibilityFormatter = accessibilityFormatter;
            _nameFormatter = nameFormatter;
            _xmldocFormatter = xmldocFormatter;
        }

        /// <summary>
        /// Formats an enumeration.
        /// </summary>
        /// <param name="type">The enumeration type to format.</param>
        /// <param name="xmldoc">The XML documentation. May be <c>null</c>.</param>
        public EnumEntity Enum(TypeDefinition type, XContainer xmldoc)
        {
            var underlyingType = type.GetEnumUnderlyingType();
            if (underlyingType.FullName == "System.Int32")
                underlyingType = null;
            return new EnumEntity
            {
                DnaId = type.DnaId(),
                Attributes = _attributeFormatter.Attributes(type).ToList(),
                Accessibility = _accessibilityFormatter.TypeDefinitionAccessibility(type),
                Name = _nameFormatter.EscapeIdentifier(type.Name),
                Namespace = type.IsNested ? null : type.Namespace,
                UnderlyingTypeDnaId = underlyingType?.DnaId(),
                PreferHex = type.EnumHasFlagsAttribute(),
                Fields = type.Fields.Where(x => x.IsStatic).Select(field => new EnumField
                {
                    Name = _nameFormatter.EscapeIdentifier(field.Name),
                    Value = field.Constant,
                    Xmldoc = _xmldocFormatter.Xmldoc(field, xmldoc),
                }).ToList(),
                Xmldoc = _xmldocFormatter.Xmldoc(type, xmldoc),
            };
        }
    }
}
