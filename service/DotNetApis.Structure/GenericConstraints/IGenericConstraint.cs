using Newtonsoft.Json;

namespace DotNetApis.Structure.GenericConstraints
{
    public interface IGenericConstraint
    {
        [JsonProperty("k")]
        GenericConstraintKind Kind { get; }
    }
}
