using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Literals
{
    /// <summary>
    /// This must be kept in sync with constants\entityLiteralKinds.js
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum EntityLiteralKind
    {
        Null = 0,
        Primitive,
        Array,
        Typeof,
        Enum
    }
}
