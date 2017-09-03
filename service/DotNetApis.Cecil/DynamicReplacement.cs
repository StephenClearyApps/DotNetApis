using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    /// <summary>
    /// Used to determine which types should be treated as <c>dynamic</c>.
    /// </summary>
    public sealed class DynamicReplacement
    {
        private int _nextIndex;
        private readonly bool[] _values;

        private DynamicReplacement()
        {
            _values = null;
        }

        private DynamicReplacement(int nextIndex)
        {
            _nextIndex = nextIndex;
        }

        public DynamicReplacement(bool[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Gets an instance that never replaces types with <c>dynamic</c>.
        /// </summary>
        public static DynamicReplacement NoDynamic { get; } = new DynamicReplacement();

        /// <summary>
        /// Gets an instance that always replaces types with <c>dynamic</c> (in practice, this results in a single, top-level <c>dynamic</c>).
        /// </summary>
        public static DynamicReplacement SingleDynamic { get; } = new DynamicReplacement(-1);

        /// <summary>
        /// Whether the current type should be replaced with <c>dynamic</c>. This method must be called in a prefix traversal of a type's construction.
        /// </summary>
        /// <returns></returns>
        public bool CheckDynamicAndIncrement()
        {
            if (_nextIndex < 0)
                return true;
            if (_values == null || _nextIndex >= _values.Length)
                return false;
            var result = _values[_nextIndex];
            ++_nextIndex;
            return result;
        }
    }
    
    public static partial class CecilExtensions
    {
        /// <summary>
        /// Gets the <see cref="DynamicReplacement"/> for this declaration.
        /// </summary>
        public static DynamicReplacement GetDynamicReplacement(this ICustomAttributeProvider @this)
        {
            var attribute = @this.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.DynamicAttribute");
            if (attribute == null)
                return DynamicReplacement.NoDynamic;
            if (attribute.ConstructorArguments.Count == 0)
                return DynamicReplacement.SingleDynamic;
            return new DynamicReplacement(((CustomAttributeArgument[])attribute.ConstructorArguments[0].Value).Select(x => (bool)x.Value).ToArray());
        }
    }
}
