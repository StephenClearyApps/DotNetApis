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
    public sealed class DelegateFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly AccessibilityFormatter _accessibilityFormatter;
        private readonly NameFormatter _nameFormatter;
        private readonly TypeReferenceFormatter _typeReferenceFormatter;
        private readonly GenericsFormatter _genericsFormatter;
        private readonly MethodFormatter _methodFormatter;
        private readonly XmldocFormatter _xmldocFormatter;

        public DelegateFormatter(AttributeFormatter attributeFormatter, AccessibilityFormatter accessibilityFormatter, NameFormatter nameFormatter, TypeReferenceFormatter typeReferenceFormatter,
            GenericsFormatter genericsFormatter, MethodFormatter methodFormatter, XmldocFormatter xmldocFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _accessibilityFormatter = accessibilityFormatter;
            _nameFormatter = nameFormatter;
            _typeReferenceFormatter = typeReferenceFormatter;
            _genericsFormatter = genericsFormatter;
            _methodFormatter = methodFormatter;
            _xmldocFormatter = xmldocFormatter;
        }

        /// <summary>
        /// Formats a delegate.
        /// </summary>
        /// <param name="type">The delegate to format.</param>
        /// <param name="xmldoc">The XML documentation. May be <c>null</c>.</param>
        public DelegateEntity Delegate(TypeDefinition type, XContainer xmldoc)
        {
            var method = type.Methods.First(x => x.Name == "Invoke");

            // The generic parameters for a delegate are stored on the delegate type itself, rather than its Invoke method.
            // In other words, it's stored like "class MyDelegate<T> { Invoke(); }", not like "class MyDelegate { Invoke<T>(); }".
            var typeWithGenericParameters = type.GenericDeclaringTypesAndThis().Last();

            return new DelegateEntity
            {
                DnaId = type.DnaId(),
                Attributes = _attributeFormatter.Attributes(type).Concat(_attributeFormatter.Attributes(method.MethodReturnType, "return")).ToList(),
                Accessibility = _accessibilityFormatter.TypeDefinitionAccessibility(type),
                Name = _nameFormatter.EscapeIdentifier(typeWithGenericParameters.Name),
                Namespace = type.IsNested ? null : type.Namespace,
                ReturnType = _typeReferenceFormatter.TypeReference(method.ReturnType, method.MethodReturnType.GetDynamicReplacement()),
                GenericParameters = _genericsFormatter.GenericParameters(type, typeWithGenericParameters.GenericParameters, xmldoc).ToList(),
                Parameters = _methodFormatter.Parameters(type, method.Parameters, xmldoc).ToList(),
                Xmldoc = _xmldocFormatter.Xmldoc(type, xmldoc),
            };
        }
    }
}
