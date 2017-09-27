using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Common.LogStructure
{
    /// <summary>
    /// Serializes timestamps as milliseconds since the Unix epoch. Note: milliseconds, not seconds.
    /// </summary>
    public sealed class EpochTimestampJsonConverter : JsonConverter
    {
        private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public override bool CanConvert(Type objectType) => objectType == typeof(DateTimeOffset);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dto = (DateTimeOffset)value;
            var delta = dto - Epoch;
            writer.WriteValue((long)delta.TotalMilliseconds);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var ticks = (long)reader.Value;
            return Epoch.AddMilliseconds(ticks);
        }
    }
}
