using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A constant array.
    /// </summary>
    public sealed class ArrayLiteral : ILiteral
    {
        public EntityLiteralKind Kind => EntityLiteralKind.Array;

        /// <summary>
        /// The type of the array elements.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference ElementType { get; set; }

        /// <summary>
        /// The array element values.
        /// </summary>
        [JsonProperty("v")]
        public IReadOnlyList<ILiteral> Values { get; set; }
    }
}
