using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// The kind of entity this structured entry represents.
    /// This must be kept in sync with $FrontEnd/src/constants/entityKinds.js
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum EntityKind
    {
        Class = 0,
        Interface,
        Struct,
        Enum,
        Delegate,
        Method,
        Property,
        Event,
        Field,
    }
}
