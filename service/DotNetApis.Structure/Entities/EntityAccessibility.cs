using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// The accessibility of an entity.
    /// This must be kept in sync with constants/entityAccessibility.js
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum EntityAccessibility
    {
        Public = 0,
        Protected = 1,
        Hidden = 2,
    }
}
