using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// This must be kept in sync with constants/entityMethodStyles.js
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum MethodStyles
    {
        None = 0,
        Extension = 1,
        Implicit = 2,
        Explicit = 3,
        Operator = 4,
    }
}
