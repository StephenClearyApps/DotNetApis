using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    public sealed class AttributeFormatter
    {
        private readonly ILogger _logger;
        private readonly NameFormatter _nameFormatter;
        private readonly TypeLocator _typeLocator;
        private readonly LiteralFormatter _literalFormatter;

        private static readonly HashSet<string> IgnoredAttributes = new HashSet<string>
        {
            "System.ParamArrayAttribute",
            "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
            "System.Runtime.CompilerServices.DecimalConstantAttribute",
            "System.Runtime.CompilerServices.ExtensionAttribute",
            "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute",
            "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute",
            "System.Diagnostics.DebuggerDisplayAttribute",
            "System.Diagnostics.DebuggerTypeProxyAttribute",
            "System.Diagnostics.DebuggerStepThroughAttribute",
            "System.Diagnostics.DebuggerVisualizerAttribute",
            "System.Runtime.CompilerServices.AsyncStateMachineAttribute",
            "System.Runtime.CompilerServices.IteratorStateMachineAttribute",
            "System.Runtime.CompilerServices.InternalsVisibleToAttribute",
        };

        public AttributeFormatter(ILogger logger, NameFormatter nameFormatter, TypeLocator typeLocator, LiteralFormatter literalFormatter)
        {
            _logger = logger;
            _nameFormatter = nameFormatter;
            _typeLocator = typeLocator;
            _literalFormatter = literalFormatter;
        }

        /// <summary>
        /// Formats the custom attributes (with their arguments) for the specified target.
        /// </summary>
        /// <param name="type">The type whose custom attributes will be appended.</param>
        /// <param name="target">The target for the custom attributes. This may be <c>null</c> to indicate the default target.</param>
        public IEnumerable<AttributeJson> Attributes(ICustomAttributeProvider type, string target = null) => FilterAttributes(type).Select(x => Attribute(x, target));

        /// <summary>
        /// Sorts and then filters out custom attributes that we prefer not to display. Removes all <see cref="IgnoredAttributes"/> as well as [Dynamic] (unless applied to a type).
        /// </summary>
        /// <param name="type">The type whose custom attributes will be filtered.</param>
        private static IEnumerable<CustomAttribute> FilterAttributes(ICustomAttributeProvider type)
        {
            foreach (var attribute in type.CustomAttributes.OrderBy(x => x.AttributeType.Name))
            {
                if (IgnoredAttributes.Contains(attribute.AttributeType.FullName))
                    continue;
                if (attribute.AttributeType.FullName == "System.Runtime.CompilerServices.DynamicAttribute" && !(type is TypeReference))
                    continue;
                yield return attribute;
            }
        }

        /// <summary>
        /// Appends the custom attribute (with its arguments) for the specified target.
        /// </summary>
        /// <param name="attribute">The custom attribute to append.</param>
        /// <param name="target">The target for this custom attribute. This may be <c>null</c> to indicate the default target.</param>
        private AttributeJson Attribute(CustomAttribute attribute, string target = null)
        {
            if (attribute.AttributeType.Resolve() == null)
                _logger.LogWarning("Unable to resolve custom attribute type {dnaid}", attribute.AttributeType.DnaId());
            var name = attribute.AttributeType.Name;
            if (name.EndsWith("Attribute"))
                name = name.Substring(0, name.Length - 9);
            return new AttributeJson
            {
                Target = target,
                Name = _nameFormatter.EscapeIdentifier(name),
                Location = _typeLocator.TryGetLocationFromDnaId(attribute.AttributeType.DnaId()),
                Arguments = attribute.AttributeConstructorArguments().Select(AttributeConstructorArgument).ToList(),
            };
        }

        /// <summary>
        /// Appends the custom attribute argument (with its value).
        /// </summary>
        /// <param name="argument">The attribute argument.</param>
        private AttributeArgumentJson AttributeConstructorArgument(AttributeConstructorArgument argument)
        {
            return new AttributeArgumentJson
            {
                Name = _nameFormatter.EscapeIdentifier(argument.Name), // May be null
                Value = _literalFormatter.Literal(argument.Type, argument.Value),
            };
        }
    }
}
