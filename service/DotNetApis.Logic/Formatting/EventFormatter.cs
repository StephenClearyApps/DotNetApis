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
    public sealed class EventFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly AccessibilityFormatter _accessibilityFormatter;
        private readonly ModifiersFormatter _modifiersFormatter;
        private readonly TypeReferenceFormatter _typeReferenceFormatter;
        private readonly NameFormatter _nameFormatter;
        private readonly XmldocFormatter _xmldocFormatter;

        public EventFormatter(AttributeFormatter attributeFormatter, AccessibilityFormatter accessibilityFormatter, ModifiersFormatter modifiersFormatter,
            TypeReferenceFormatter typeReferenceFormatter, NameFormatter nameFormatter, XmldocFormatter xmldocFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _accessibilityFormatter = accessibilityFormatter;
            _modifiersFormatter = modifiersFormatter;
            _typeReferenceFormatter = typeReferenceFormatter;
            _nameFormatter = nameFormatter;
            _xmldocFormatter = xmldocFormatter;
        }

        /// <summary>
        /// Formats an event declaration.
        /// </summary>
        /// <param name="event">The event to format.</param>
        public EventEntity Event(EventDefinition @event)
        {
            var result = new EventEntity
            {
                DnaId = @event.DnaId(),
                Accessibility = EntityAccessibility.Hidden,
                Type = _typeReferenceFormatter.TypeReference(@event.AddMethod.Parameters[0].ParameterType, @event.AddMethod.Parameters[0].GetDynamicReplacement()),
                Xmldoc = _xmldocFormatter.Xmldoc(@event),
            };

            var field = @event.TryGetField();
            result.Attributes = field == null ?
                _attributeFormatter.Attributes(@event).ToList() :
                _attributeFormatter.Attributes(@event).Concat(_attributeFormatter.Attributes(@event.AddMethod, "method")).Concat(_attributeFormatter.Attributes(field, "field")).ToList();

            var explicitInterfaceMethod = @event.AddMethod.GetExplicitlyImplementedInterfaceMethod();
            if (!@event.DeclaringType.IsInterface && explicitInterfaceMethod == null)
            {
                result.Accessibility = _accessibilityFormatter.MethodAccessibility(@event.AddMethod);
                result.Modifiers = _modifiersFormatter.MethodModifiers(@event.AddMethod);
            }

            var eventName = @event.Name;
            if (explicitInterfaceMethod != null)
            {
                result.ExplicitInterfaceDeclaringType = _typeReferenceFormatter.TypeReference(explicitInterfaceMethod.DeclaringType);
                eventName = eventName.Substring(eventName.LastIndexOf('.') + 1);
            }
            result.Name = _nameFormatter.EscapeIdentifier(eventName);

            if (field == null && !@event.DeclaringType.IsInterface)
            {
                // If there's no attributes to display, then there's not any point in sticking the add/remove on here.
                if (@event.AddMethod.HasCustomAttributes || @event.RemoveMethod.HasCustomAttributes ||
                    @event.AddMethod.MethodReturnType.HasCustomAttributes || @event.RemoveMethod.MethodReturnType.HasCustomAttributes ||
                    @event.AddMethod.Parameters[0].HasCustomAttributes || @event.RemoveMethod.Parameters[0].HasCustomAttributes)
                {
                    result.AddMethodAttributes = _attributeFormatter.Attributes(@event.AddMethod)
                        .Concat(_attributeFormatter.Attributes(@event.AddMethod.MethodReturnType, "return"))
                        .Concat(_attributeFormatter.Attributes(@event.AddMethod.Parameters[0], "param")).ToList();
                    result.RemoveMethodAttributes = _attributeFormatter.Attributes(@event.RemoveMethod)
                        .Concat(_attributeFormatter.Attributes(@event.RemoveMethod.MethodReturnType, "return"))
                        .Concat(_attributeFormatter.Attributes(@event.RemoveMethod.Parameters[0], "param")).ToList();
                }
            }
            return result;
        }
    }
}
