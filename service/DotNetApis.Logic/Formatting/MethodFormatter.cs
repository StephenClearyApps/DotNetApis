using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Common;
using DotNetApis.Structure.Entities;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    public sealed class MethodFormatter
    {
        /// <summary>
        /// All C# operators that can be overridden. This does not include implicit or explicit conversions, or CLI operator special names not supported by C#.
        /// </summary>
        private static readonly Dictionary<string, string> KnownCsharpOverridableOperatorNames = new Dictionary<string, string>
        {
            { "op_UnaryPlus", "+" },
            { "op_UnaryNegation", "-" },
            { "op_LogicalNot", "!" },
            { "op_OnesComplement", "~" },
            { "op_Increment", "++" },
            { "op_Decrement", "--" },
            { "op_True", "true" },
            { "op_False", "false" },
            { "op_Addition", "+" },
            { "op_Subtraction", "+" },
            { "op_Multiply", "*" },
            { "op_Division", "/" },
            { "op_Modulus", "%" },
            { "op_BitwiseAnd", "&" },
            { "op_BitwiseOr", "|" },
            { "op_ExclusiveOr", "^" },
            { "op_LeftShift", "<<" },
            { "op_RightShift", ">>" },
            { "op_Equality", "==" },
            { "op_Inequality", "!=" },
            { "op_LessThan", "<" },
            { "op_GreaterThan", ">" },
            { "op_LessThanOrEqual", "<=" },
            { "op_GreaterThanOrEqual", ">=" },
        };

        private readonly AttributeFormatter _attributeFormatter;
        private readonly TypeReferenceFormatter _typeReferenceFormatter;
        private readonly NameFormatter _nameFormatter;
        private readonly LiteralFormatter _literalFormatter;
        private readonly XmldocFormatter _xmldocFormatter;
        private readonly AccessibilityFormatter _accessibilityFormatter;
        private readonly ModifiersFormatter _modifiersFormatter;
        private readonly GenericsFormatter _genericsFormatter;

        public MethodFormatter(AttributeFormatter attributeFormatter, TypeReferenceFormatter typeReferenceFormatter, NameFormatter nameFormatter, LiteralFormatter literalFormatter,
            XmldocFormatter xmldocFormatter, AccessibilityFormatter accessibilityFormatter, ModifiersFormatter modifiersFormatter, GenericsFormatter genericsFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _typeReferenceFormatter = typeReferenceFormatter;
            _nameFormatter = nameFormatter;
            _literalFormatter = literalFormatter;
            _xmldocFormatter = xmldocFormatter;
            _accessibilityFormatter = accessibilityFormatter;
            _modifiersFormatter = modifiersFormatter;
            _genericsFormatter = genericsFormatter;
        }

        // TODO: Check for ToArray(); check for "Structured" in method names.

	    /// <summary>
	    /// Formats a method declaration, which may be a method, operator, constructor, destructor, or type constructor.
	    /// </summary>
	    /// <param name="method">The method to format.</param>
	    public MethodEntity Method(MethodDefinition method)
	    {
		    var result = new MethodEntity
		    {
			    DnaId = method.DnaId(),
			    Attributes = _attributeFormatter.Attributes(method).Concat(_attributeFormatter.Attributes(method.MethodReturnType, "return")).ToList(),
			    Parameters = Parameters(method, method.Parameters).ToList(),
			    Accessibility = EntityAccessibility.Hidden,
			    Xmldoc = _xmldocFormatter.Xmldoc(method),
		    };

		    var methodName = method.Name.StripBacktickSuffix().Name;
		    var isStaticConstructor = methodName == ".cctor";
		    var isFinalizer = methodName == "Finalize";
		    var explicitInterfaceMethod = method.GetExplicitlyImplementedInterfaceMethod();
		    var hasModifiers = !isFinalizer && !method.DeclaringType.IsInterface && explicitInterfaceMethod == null;

		    if (hasModifiers && !isStaticConstructor)
			    result.Accessibility = _accessibilityFormatter.MethodAccessibility(method);
		    if (hasModifiers)
			    result.Modifiers = _modifiersFormatter.MethodModifiers(method);

		    if (methodName == "op_Implicit" || methodName == "op_Explicit")
		    {
			    result.ReturnType = _typeReferenceFormatter.TypeReference(method.ReturnType, method.MethodReturnType.GetDynamicReplacement());
			    result.Styles = methodName == "op_Implicit" ? MethodStyles.Implicit : MethodStyles.Explicit;
			    return result;
		    }

		    var isOperator = false;
		    if (methodName == ".ctor" || isStaticConstructor || isFinalizer)
		    {
			    methodName = method.SimpleMethodName();
		    }
		    else if (KnownCsharpOverridableOperatorNames.ContainsKey(methodName))
            {
                isOperator = true;
                result.ReturnType = _typeReferenceFormatter.TypeReference(method.ReturnType, method.MethodReturnType.GetDynamicReplacement());
                methodName = KnownCsharpOverridableOperatorNames[methodName];
            }
            else
            {
                result.ReturnType = _typeReferenceFormatter.TypeReference(method.ReturnType, method.MethodReturnType.GetDynamicReplacement());
            }

            if (explicitInterfaceMethod != null)
            {
                result.ExplicitInterfaceDeclaringType = _typeReferenceFormatter.TypeReference(explicitInterfaceMethod.DeclaringType);
                methodName = explicitInterfaceMethod.Name.StripBacktickSuffix().Name;
            }

            result.Name = _nameFormatter.EscapeIdentifier(methodName);
            result.Styles = isOperator ? MethodStyles.Operator :
                method.IsExtensionMethod() ? MethodStyles.Extension :
                MethodStyles.None;
            result.GenericParameters = _genericsFormatter.GenericParameters(method, method.GenericParameters).ToList();
            return result;
        }

        /// <summary>
        /// Formats method parameters.
        /// </summary>
        /// <param name="member">The member whose parameters these are.</param>
        /// <param name="parameters">The parameters to format.</param>
        public IEnumerable<MethodParameter> Parameters(IMemberDefinition member, IEnumerable<ParameterDefinition> parameters) =>
            parameters.Select(p => Parameter(member, p));

        /// <summary>
        /// Formats a method parameter.
        /// </summary>
        /// <param name="member">The member whose parameter this is.</param>
        /// <param name="parameter">The parameter to format.</param>
        private MethodParameter Parameter(IMemberDefinition member, ParameterDefinition parameter)
        {
            var parameterType = parameter.ParameterType;
            var byRefParameterType = parameterType as ByReferenceType;
            if (byRefParameterType != null)
                parameterType = ((ByReferenceType)parameterType).ElementType;
            var decimalConstantAttribute = parameter.TryGetDecimalConstantAttribute();

            var modifiers = byRefParameterType != null && parameter.IsOut ? MethodParameterModifiers.Out :
                byRefParameterType != null && !parameter.IsOut ? MethodParameterModifiers.Ref :
                parameter.CustomAttributes.Any(x => x.AttributeType.FullName == "System.ParamArrayAttribute") ? MethodParameterModifiers.Params :
                MethodParameterModifiers.None;
            var value = parameter.HasConstant ? _literalFormatter.Literal(parameter.ParameterType, parameter.Constant) :
                decimalConstantAttribute != null ? _literalFormatter.Literal(parameter.ParameterType, decimalConstantAttribute.GetDecimalValue()) :
                null;

            return new MethodParameter
            {
                Attributes = _attributeFormatter.Attributes(parameter).ToList(),
                Modifiers = modifiers,
                Type = _typeReferenceFormatter.TypeReference(parameterType, parameter.GetDynamicReplacement()),
                Name = _nameFormatter.EscapeIdentifier(parameter.Name),
                Value = value,
                XmldocNode = _xmldocFormatter.XmldocNodeForParameter(member, parameter),
            };
        }
    }
}
