using System;
using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// Modifiers on entities.
    /// Must be kept in sync with util/structure/entities.ts
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    [Flags]
    public enum EntityModifiers
    {
        None = 0,
        Static = 0x1,
        Abstract = 0x2,
        Sealed = 0x4,
        Virtual = 0x8,
        Override = 0x10,
        Const = 0x20,
    }
}
