using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// This must be kept in sync with constants/entityGenericConstraintsKinds.js
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum GenericConstraintKind
    {
        Class = 0,
        Struct,
        New,
        Type,
    }
}
