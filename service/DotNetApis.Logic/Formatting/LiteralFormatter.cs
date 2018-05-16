using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Common;
using DotNetApis.Structure.Literals;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace DotNetApis.Logic.Formatting
{
    /// <summary>
    /// Formats literal (constant) values.
    /// </summary>
    public sealed class LiteralFormatter
    {
        private readonly ILogger<LiteralFormatter> _logger;
        private readonly TypeReferenceFormatter _typeReferenceFormatter;

        public LiteralFormatter(ILoggerFactory loggerFactory, TypeReferenceFormatter typeReferenceFormatter)
        {
            _logger = loggerFactory.CreateLogger<LiteralFormatter>();
            _typeReferenceFormatter = typeReferenceFormatter;
        }

        /// <summary>
        /// Formats a literal value.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="value">The value. This must be <c>null</c>, an integral value, a fractional value, a type reference, a boolean, an enumeration, or an array of one of these types.</param>
        /// <param name="preferHex">Whether to prefer hexadecimal formatting if the value is integral.</param>
        public ILiteral Literal(TypeReference type, object value, bool preferHex = false)
        {
            while (value is CustomAttributeArgument)
            {
                type = ((CustomAttributeArgument)value).Type;
                value = ((CustomAttributeArgument)value).Value;
            }

            if (value == null)
                return new NullLiteral();

            if (type is ArrayType arrayType)
            {
                var array = (Array)value;
                return new ArrayLiteral
                {
                    ElementType = _typeReferenceFormatter.TypeReference(arrayType.ElementType),
                    Values = array.Cast<object>().Select(x => Literal(arrayType.ElementType, x, preferHex)).ToList(),
                };
            }

            if (type.Namespace == "System" && type.Name == "Nullable`1")
                type = ((GenericInstanceType)type).GenericArguments[0];

            if (type.Namespace == "System")
            {
                if (type.Name == "Type")
                {
                    return new TypeofLiteral
                    {
                        Type = _typeReferenceFormatter.TypeReference((TypeReference)value),
                    };
                }
                if (type.Name == "String" || type.Name == "Boolean" || type.Name == "Char" ||
                    type.Name == "Byte" || type.Name == "SByte" || type.Name == "Int16" || type.Name == "UInt16" || type.Name == "Int32" || type.Name == "UInt32" ||
                    type.Name == "Int64" || type.Name == "UInt64" ||
                    type.Name == "Single" || type.Name == "Double" || type.Name == "Decimal")
                {
                    return new PrimitiveLiteral
                    {
                        Value = value,
                        PreferHex = preferHex,
                    };
                }
            }

            // Treat the value as an enumeration.

            var definition = type.Resolve();
            if (!definition.IsEnum)
            {
                _logger.UnknownLiteralValueType(type.FullName);
                throw new NotImplementedException($"Unknown literal value type {type.FullName}");
            }
            var enumValues = definition.Fields.Where(x => x.IsStatic).ToList();

            preferHex = definition.EnumHasFlagsAttribute();
            var enumType = _typeReferenceFormatter.TypeReference(type);

            // First, see if there's an exact match.
            foreach (var enumValue in enumValues)
            {
                if (value.Equals(enumValue.Constant))
                {
                    return new EnumLiteral
                    {
                        Value = value,
                        PreferHex = preferHex,
                        EnumType = enumType,
                        Names = new[] { enumValue.Name },
                    };
                }
            }

            // Next, try to build the value out of the defined members, even if the flags attribute isn't specified.

            // Special-case zero.
            if (value.Equals(0))
            {
                return new EnumLiteral
                {
                    Value = value,
                    PreferHex = preferHex,
                    EnumType = enumType,
                };
            }

            var enumIntegralType = definition.GetEnumUnderlyingType();
            var integralValue = GetIntegralValue(enumIntegralType, value);
            var components = TryMatchValue(integralValue, enumIntegralType, enumValues);

            return new EnumLiteral
            {
                Value = value,
                PreferHex = preferHex,
                EnumType = enumType,
                Names = components?.Select(x => x.Field.Name)?.ToList(),
            };
        }

        /// <summary>
        /// A value from an enumeration.
        /// </summary>
        private struct EnumValue
        {
            public EnumValue(FieldDefinition field, TypeReference underlyingType) : this()
            {
                Field = field;
                Value = GetIntegralValue(underlyingType, field.Constant);
                Population = PopulationCount(Value);
            }

            /// <summary>
            /// The reference to the enumeration value.
            /// </summary>
            public FieldDefinition Field { get; }

            /// <summary>
            /// The integral value of the enumeration value.
            /// </summary>
            public ulong Value { get; }

            /// <summary>
            /// The population count of the integral value.
            /// </summary>
            public int Population { get; }
        }

        /// <summary>
        /// Attempts to deconstruct an integral value into a series of enumeration values that, when OR'ed together, result in the original value. Returns <c>null</c> if no such deconstruction could be found.
        /// </summary>
        /// <param name="value">The original integral value.</param>
        /// <param name="type">The underlying type of the enumeration.</param>
        /// <param name="enumValues">The enumeration values.</param>
        private static List<EnumValue> TryMatchValue(ulong value, TypeReference type, IEnumerable<FieldDefinition> enumValues)
        {
            var result = new List<EnumValue>();
            foreach (var enumValue in enumValues.Select(x => new EnumValue(x, type)).Where(x => x.Value != 0).OrderByDescending(x => x.Population))
            {
                if ((value & enumValue.Value) == enumValue.Value)
                {
                    value &= ~enumValue.Value;
                    result.Add(enumValue);
                }
            }
            if (value != 0)
                return null;
            return result;
        }

        /// <summary>
        /// Returns the number of set bits in a value.
        /// </summary>
        /// <param name="value">The value to examine.</param>
        private static int PopulationCount(ulong value)
        {
            var result = 0;
            while (value != 0)
            {
                if ((value & 0x1) == 0x1)
                    ++result;
                value >>= 1;
            }
            return result;
        }

        /// <summary>
        /// Unboxes an integral value.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="value">The value to unbox. This must be a signed or unsigned 8, 16, 32, or 64-bit number.</param>
        private static ulong GetIntegralValue(TypeReference type, object value)
        {
            if (type.FullName == "System.Byte")
                return (byte)value;
            if (type.FullName == "System.Sbyte")
                return unchecked((byte)(sbyte)value);
            if (type.FullName == "System.Int16")
                return unchecked((ushort)(short)value);
            if (type.FullName == "System.UInt16")
                return (ushort)value;
            if (type.FullName == "System.Int32")
                return unchecked((uint)(int)value);
            if (type.FullName == "System.UInt32")
                return (uint)value;
            if (type.FullName == "System.Int64")
                return unchecked((ulong)(long)value);
            return (ulong)value;
        }
    }

    internal static partial class Logging
    {
        public static void UnknownLiteralValueType(this ILogger<LiteralFormatter> logger, string type) =>
            Logger.Log(logger, 1, LogLevel.Critical, "Unknown literal value type {type}", type, null);
    }
}
