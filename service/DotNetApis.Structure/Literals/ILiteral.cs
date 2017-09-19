using Newtonsoft.Json;

namespace DotNetApis.Structure.Literals
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
