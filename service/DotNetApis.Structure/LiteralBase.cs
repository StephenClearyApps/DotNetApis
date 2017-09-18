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
    public abstract class LiteralBase
    {
        /// <summary>
        /// The kind of literal this is.
        /// </summary>
        [JsonProperty("k")]
        public abstract EntityLiteralKind Kind { get; }
    }
}
