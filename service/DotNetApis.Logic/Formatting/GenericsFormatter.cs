using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Structure.Entities;
using DotNetApis.Structure.GenericConstraints;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    /// <summary>
    /// Formats generic parameters and their constraints.
    /// </summary>
    public sealed class GenericsFormatter
    {
        private readonly TypeReferenceFormatter _typeReferenceFormatter;
        private readonly XmldocFormatter _xmldocFormatter;
        private readonly NameFormatter _nameFormatter;

        public GenericsFormatter(TypeReferenceFormatter typeReferenceFormatter, XmldocFormatter xmldocFormatter, NameFormatter nameFormatter)
        {
            _typeReferenceFormatter = typeReferenceFormatter;
            _xmldocFormatter = xmldocFormatter;
            _nameFormatter = nameFormatter;
        }

        /// <summary>
        /// Formats a list of generic parameters.
        /// </summary>
        /// <param name="member">The member defining this generic parameter.</param>
        /// <param name="parameters">The generic parameters to format.</param>
        /// <param name="xmldoc">The XML documentation.</param>
        public IReadOnlyList<GenericParameterJson> StructuredGenericParameters(IMemberDefinition member, IEnumerable<GenericParameter> parameters, XContainer xmldoc)
        {
            return parameters.Select(x => StructuredGenericParameter(member, x, xmldoc)).ToList();
        }

        /// <summary>
        /// Formats a single generic parameter.
        /// </summary>
        /// <param name="member">The member defining this generic parameter.</param>
        /// <param name="parameter">The generic parameter to format.</param>
        /// <param name="xmldoc">The XML documentation.</param>
        private GenericParameterJson StructuredGenericParameter(IMemberDefinition member, GenericParameter parameter, XContainer xmldoc)
        {
            return new GenericParameterJson
            {
                Modifiers = parameter.IsContravariant ? GenericParameterModifiers.In :
                    parameter.IsCovariant ? GenericParameterModifiers.Out :
                    GenericParameterModifiers.Invariant,
                Name = _nameFormatter.EscapeIdentifier(parameter.Name),
                GenericConstraints = parameter.GenericConstraints().Select(GenericConstraint).ToList(),
                XmldocNode = _xmldocFormatter.XmldocNode(member, parameter, xmldoc),
            };
        }

        /// <summary>
        /// Formats a generic constraint.
        /// </summary>
        /// <param name="constraint">The constraint to format.</param>
        private IGenericConstraint GenericConstraint(GenericConstraint constraint)
        {
            if (constraint.Type == GenericConstraintType.DefaultConstructor)
                return new NewGenericConstraint();
            if (constraint.Type == GenericConstraintType.NonNullableValueType)
                return new StructGenericConstraint();
            if (constraint.Type == GenericConstraintType.ReferenceType)
                return new ClassGenericConstraint();
            return new TypeGenericConstraint
            {
                Type = _typeReferenceFormatter.TypeReference(constraint.TypeReference),
            };
        }
    }
}
