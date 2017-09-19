using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A literal (constant) value.
    /// </summary>
    public interface ILiteral
    {
        /// <summary>
        /// The kind of literal this is.
        /// </summary>
        [JsonProperty("k")]
        EntityLiteralKind Kind { get; }
    }
}
