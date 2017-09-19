using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Literals
{
    /// <summary>
    /// This must be kept in sync with constants\entityLiteralKinds.js
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum LiteralKind
    {
        Null = 0,
        Primitive,
        Array,
        Typeof,
        Enum
    }
}
