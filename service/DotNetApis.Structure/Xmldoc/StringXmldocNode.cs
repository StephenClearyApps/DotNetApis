using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetApis.Structure.Xmldoc
{
    /// <summary>
    /// An xmldoc node that is a plain string.
    /// </summary>
    [JsonConverter(typeof(StringXmldocNodeJsonConverter))]
    public sealed class StringXmldocNode : IXmldocNode
    {
        /// <summary>
        /// The text of the xmldoc node.
        /// </summary>
        public string Text { get; set; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class StringXmldocNodeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(StringXmldocNodeJsonConverter);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new StringXmldocNode
            {
                Text = JToken.Load(reader).Value<string>(),
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => JToken.FromObject(((StringXmldocNode) value).Text).WriteTo(writer);
    }
}
