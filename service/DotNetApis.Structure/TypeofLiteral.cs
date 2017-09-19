using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A literal "typeof(T)" expression.
    /// </summary>
    public sealed class TypeofLiteral : ILiteral
    {
        public EntityLiteralKind Kind => EntityLiteralKind.Typeof;

        /// <summary>
        /// The type being referred to.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference Type { get; set; }
    }
}
