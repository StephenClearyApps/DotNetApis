﻿using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// This must be kept in sync with util/structure/entities.ts
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum MethodParameterModifiers
    {
        None = 0,
        Params = 1,
        Ref = 2,
        Out = 3,
    }
}
