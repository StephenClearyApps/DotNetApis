using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Structure.Entities;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    public sealed class PropertyFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly AccessibilityFormatter _accessibilityFormatter;
        private readonly ModifiersFormatter _modifiersFormatter;
        private readonly TypeReferenceFormatter _typeReferenceFormatter;
        private readonly NameFormatter _nameFormatter;
        private readonly MethodFormatter _methodFormatter;
        private readonly XmldocFormatter _xmldocFormatter;

        public PropertyFormatter(AttributeFormatter attributeFormatter, AccessibilityFormatter accessibilityFormatter, ModifiersFormatter modifiersFormatter,
            TypeReferenceFormatter typeReferenceFormatter, NameFormatter nameFormatter, MethodFormatter methodFormatter, XmldocFormatter xmldocFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _accessibilityFormatter = accessibilityFormatter;
            _modifiersFormatter = modifiersFormatter;
            _typeReferenceFormatter = typeReferenceFormatter;
            _nameFormatter = nameFormatter;
            _methodFormatter = methodFormatter;
            _xmldocFormatter = xmldocFormatter;
        }

        /// <summary>
        /// Formats a property declaration, which may be a property or an indexer.
        /// </summary>
        /// <param name="property">The property to format.</param>
        /// <param name="xmldoc">The XML documentation. May be <c>null</c>.</param>
        public PropertyEntity Property(PropertyDefinition property, XContainer xmldoc)
        {
            var result = new PropertyEntity
            {
                DnaId = property.DnaId(),
                Attributes = _attributeFormatter.Attributes(property).ToList(),
                Accessibility = EntityAccessibility.Hidden,
                Type = _typeReferenceFormatter.TypeReference(property.GetMethod == null ?
                    property.SetMethod.Parameters.Last().ParameterType :
                    property.GetMethod.ReturnType, property.GetDynamicReplacement()),
                Xmldoc = _xmldocFormatter.Xmldoc(property, xmldoc),
            };

            var eitherMethod = property.GetMethod ?? property.SetMethod;
            var explicitInterfaceMethod = eitherMethod.GetExplicitlyImplementedInterfaceMethod();
            var propertyAccessor = AccessibilityRestrictionLevel.Private;
            if (!property.DeclaringType.IsInterface && explicitInterfaceMethod == null)
            {
                var propertyAccessibility = _accessibilityFormatter.PropertyAccessibility(property);
                propertyAccessor = propertyAccessibility.AccessibilityRestrictionLevel;
                result.Accessibility = propertyAccessibility.EntityAccessibility;
                result.Modifiers = _modifiersFormatter.MethodModifiers(eitherMethod);
            }

            var propertyName = property.Name;
            var indexerParameters = property.GetIndexerParameters();
            if (explicitInterfaceMethod != null)
            {
                result.ExplicitInterfaceDeclaringType = _typeReferenceFormatter.TypeReference(explicitInterfaceMethod.DeclaringType);
                propertyName = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
            }
            result.Name = indexerParameters.Count == 0 ? _nameFormatter.EscapeIdentifier(propertyName) : "this";
            result.Parameters = _methodFormatter.Parameters(property, indexerParameters, xmldoc).ToList();

            if (property.GetMethod != null)
            {
                var getAccessor = property.GetMethod.GetAccessibilityRestrictionLevel();
                if (getAccessor != AccessibilityRestrictionLevel.Internal && getAccessor != AccessibilityRestrictionLevel.Private)
                {
                    result.GetMethod = new PropertyMethod
                    {
                        Attributes = _attributeFormatter.Attributes(property.GetMethod).Concat(_attributeFormatter.Attributes(property.GetMethod.MethodReturnType, "return")).ToList(),
                        Accessibility = getAccessor > propertyAccessor ? _accessibilityFormatter.MethodAccessibility(property.GetMethod) : EntityAccessibility.Hidden,
                    };
                }
            }
            if (property.SetMethod != null)
            {
                var setAccessor = property.SetMethod.GetAccessibilityRestrictionLevel();
                if (setAccessor != AccessibilityRestrictionLevel.Internal && setAccessor != AccessibilityRestrictionLevel.Private)
                {
                    result.SetMethod = new PropertyMethod
                    {
                        Attributes = _attributeFormatter.Attributes(property.SetMethod).Concat(_attributeFormatter.Attributes(property.SetMethod.Parameters[0], "param")).ToArray(),
                        Accessibility = setAccessor > propertyAccessor ? _accessibilityFormatter.MethodAccessibility(property.SetMethod) : EntityAccessibility.Hidden,
                    };
                }
            }

            return result;
        }
    }
}
