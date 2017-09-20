using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Common;
using DotNetApis.Structure.TypeReferences;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    /// <summary>
    /// Formats type references.
    /// </summary>
    public sealed class TypeReferenceFormatter
    {
        private readonly ILogger _logger;
        private readonly NameFormatter _nameFormatter;
        private readonly TypeLocator _typeLocator;

        private static readonly Dictionary<string, string> KnownCsharpTypes = new Dictionary<string, string>
        {
            { "System.Void", "void" },
            { "System.Boolean", "bool" },
            { "System.Byte", "byte" },
            { "System.SByte", "sbyte" },
            { "System.Char", "char" },
            { "System.Int16", "short" },
            { "System.UInt16", "ushort" },
            { "System.Int32", "int" },
            { "System.UInt32", "uint" },
            { "System.Int64", "long" },
            { "System.UInt64", "ulong" },
            { "System.Single", "float" },
            { "System.Double", "double" },
            { "System.Decimal", "decimal" },
            { "System.Object", "object" },
            { "System.String", "string" },
        };

        public TypeReferenceFormatter(ILogger logger, NameFormatter nameFormatter, TypeLocator typeLocator)
        {
            _logger = logger;
            _nameFormatter = nameFormatter;
            _typeLocator = typeLocator;
        }

        /// <summary>
        /// Formats a type reference that does not use <c>dynamic</c>.
        /// </summary>
        /// <param name="type">The type reference to append.</param>
        public ITypeReference TypeReference(TypeReference type) => TypeReference(type, DynamicReplacement.NoDynamic);

        /// <summary>
        /// Formats a type reference. For generic types, this may be a closed generic type (e.g., <c>List&lt;int&gt;</c>), or an open generic type (e.g., <c>List&lt;&gt;</c>).
        /// </summary>
        /// <param name="type">The type reference to append.</param>
        /// <param name="dynamicReplacement">The <c>dynamic</c> replacements to apply.</param>
        public ITypeReference TypeReference(TypeReference type, DynamicReplacement dynamicReplacement)
        {
            if (dynamicReplacement.CheckDynamicAndIncrement())
                return new DynamicTypeReference();

            if (KnownCsharpTypes.ContainsKey(type.FullName))
            {
                if (type.Resolve() == null)
                    _logger.LogWarning("Unable to resolve type reference keyword {dnaid} ({keyword})", type.DnaId(), KnownCsharpTypes[type.FullName]);
                return new KeywordTypeReference
                {
                    Name = KnownCsharpTypes[type.FullName],
                    Location = _typeLocator.TryGetLocationFromDnaId(type.DnaId()),
                };
            }

            if (type is GenericParameter genericParameter)
            {
                return new GenericParameterTypeReference
                {
                    Name = _nameFormatter.EscapeIdentifier(genericParameter.Name),
                };
            }

            if (type is RequiredModifierType reqmodType)
            {
                if (reqmodType.ModifierType.Resolve() == null)
                    _logger.LogWarning("Unable to resolve required modifier type reference {dnaid}", reqmodType.ModifierType.DnaId());
                return new ReqmodTypeReference
                {
                    Location = _typeLocator.TryGetLocationFromDnaId(reqmodType.ModifierType.DnaId()),
                    ElementType = TypeReference(reqmodType.ElementType, dynamicReplacement),
                };
            }

            if (type is OptionalModifierType optmodType)
                return TypeReference(optmodType.ElementType, dynamicReplacement);

            if (type is PointerType pointerType)
            {
                return new PointerTypeReference
                {
                    ElementType = TypeReference(pointerType.ElementType, dynamicReplacement),
                };
            }

            if (type is ArrayType arrayType)
            {
                return new ArrayTypeReference
                {
                    ElementType = TypeReference(arrayType.ElementType, dynamicReplacement),
                    Dimensions = arrayType.Dimensions.Select(ArrayDimension).ToList(),
                };
            }

            if (type is GenericInstanceType genericInstance)
            {
                return new GenericInstanceTypeReference
                {
                    DeclaringTypesAndThis = genericInstance.ConcreteDeclaringTypesAndThis().Select(x => ConcreteTypeReference(x, dynamicReplacement)).ToList()
                };
            }

            // It's a fully-qualified reference to a type.

            if (type.Resolve() == null)
                _logger.LogWarning("Unable to resolve type reference {dnaid}", type.DnaId());
            var name = type.Name.StripBacktickSuffix();
            return new TypeTypeReference
            {
                Name = _nameFormatter.EscapeIdentifier(name.Name),
                Namespace = type.Namespace,
                DeclaringType = type.IsNested ? TypeReference(type.DeclaringType) : null,
                Location = _typeLocator.TryGetLocationFromDnaId(type.DnaId()),
                GenericArgumentCount = name.Value,
            };
        }

        /// <summary>
        /// Formats the array dimension.
        /// </summary>
        /// <param name="arrayDimension">The array dimension to append.</param>
        private static ArrayDimensionJson ArrayDimension(ArrayDimension arrayDimension)
        {
            return new ArrayDimensionJson
            {
                LowerBound = !arrayDimension.LowerBound.HasValue || arrayDimension.LowerBound.Value == 0 ? (int?)null : arrayDimension.LowerBound.Value,
                UpperBound = arrayDimension.UpperBound,
            };
        }

        /// <summary>
        /// Formats a type reference with its generic arguments. E.g., a list like <c>&lt;int, double&gt;</c> which uses the types <c>int</c> and <c>double</c>.
        /// </summary>
        /// <param name="type">The type and its generic arguments.</param>
        /// <param name="dynamicReplacement">The <c>dynamic</c> replacements to apply.</param>
        private GenericConcreteType ConcreteTypeReference(ConcreteTypeReference type, DynamicReplacement dynamicReplacement)
        {
            if (type.TypeReference.Resolve() == null)
                _logger.LogWarning("Unable to resolve concrete generic type {dnaid}", type.TypeReference.DnaId());
            return new GenericConcreteType
            {
                Name = _nameFormatter.EscapeIdentifier(type.Name),
                Location = _typeLocator.TryGetLocationFromDnaId(type.TypeReference.DnaId()),
                GenericArguments = type.GenericArguments.Select(x => TypeReference(x, dynamicReplacement)).ToList(),
            };
        }
    }
}
