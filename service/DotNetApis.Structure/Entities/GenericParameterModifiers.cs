﻿using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// This must be kept in sync with util/structure/entities.ts
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum GenericParameterModifiers
    {
        Invariant = 0,
        In = 1,
        Out = 2
    }
}
