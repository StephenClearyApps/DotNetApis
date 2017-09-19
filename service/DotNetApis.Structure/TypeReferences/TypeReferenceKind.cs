using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// This must be kept in sync with constants/entityReferenceKinds.js
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum TypeReferenceKind
    {
        Type = 0,
        Keyword,
        GenericInstance,
        Dynamic,
        GenericParameter,
        Array,
        Reqmod,
        Pointer,
    }
}
