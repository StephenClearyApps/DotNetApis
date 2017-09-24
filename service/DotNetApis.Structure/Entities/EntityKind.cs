using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// The kind of entity this structured entry represents.
    /// This must be kept in sync with util/structure/entities.ts
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
