using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A location within the current package (though possibly in a different dll).
    /// </summary>
    [JsonConverter(typeof(CurrentPackageLocationJsonConverter))]
    public sealed class CurrentPackageLocation
    {
        /// <summary>
        /// The DNA ID of the location.
        /// </summary>
        public string DnaId { get; set; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CurrentPackageLocationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(CurrentPackageLocation);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new CurrentPackageLocation
            {
                DnaId = JToken.Load(reader).Value<string>(),
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => JToken.FromObject(((CurrentPackageLocation) value).DnaId).WriteTo(writer);
    }
}
