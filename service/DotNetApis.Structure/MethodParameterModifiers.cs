using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// This must be kept in sync with constants/entityMethodParameterModifiers.js
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
