using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    /// <summary>
    /// An argument for a custom attribute. This may be a positional or named argument.
    /// </summary>
    public struct AttributeConstructorArgument
    {
        /// <summary>
        /// The name of the attribute's property set by this argument. This is <c>null</c> if this argument is a positional argument.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the value of this argument.
        /// </summary>
        public TypeReference Type { get; set; }

        /// <summary>
        /// The value of this argument.
        /// </summary>
        public object Value { get; set; }
    }

    public static partial class CecilExtensions
    {
        /// <summary>
        /// Enumerates the arguments for a custom attribute. Positional arguments are enumerated first, followed by named arguments.
        /// </summary>
        public static IEnumerable<AttributeConstructorArgument> AttributeConstructorArguments(this CustomAttribute @this)
        {
            foreach (var argument in @this.ConstructorArguments)
                yield return new AttributeConstructorArgument { Type = argument.Type, Value = argument.Value };
            foreach (var argument in @this.Properties)
                yield return new AttributeConstructorArgument { Name = argument.Name, Type = argument.Argument.Type, Value = argument.Argument.Value };
        }
    }
}
