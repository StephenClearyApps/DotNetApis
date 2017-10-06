using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DotNetApis.Common
{
    public static class Constants
    {
        public static UTF8Encoding Utf8 { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        public static SHA1Managed Sha1 { get; } = new SHA1Managed();

        public static JsonSerializerSettings StorageJsonSerializerSettings { get; } = CreateStorageJsonSerializerSettings();
        public static JsonSerializerSettings CommunicationJsonSerializerSettings { get; } = CreateCommunicationJsonSerializerSettings();

        private static JsonSerializerSettings CreateCommunicationJsonSerializerSettings()
        {
            var result = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
            };
            result.Converters.Add(new StringEnumConverter());
            return result;
        }

        private static JsonSerializerSettings CreateStorageJsonSerializerSettings()
        {
            var result = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                ContractResolver = new CustomContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
            };
            result.Converters.Add(new StringEnumConverter());
            return result;
        }

        /// <summary>
        /// This contract resolver serializes properties as camelCase and skips serialization of properties that are empty collections.
        /// </summary>
        private sealed class CustomContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var result = base.CreateProperty(member, memberSerialization);
                var property = member as PropertyInfo;
                if (property == null)
                    return result;
                result.ShouldSerialize = instance =>
                {
                    var propertyValue = property.GetValue(instance) as System.Collections.IEnumerable;
                    if (propertyValue == null || propertyValue is string)
                        return true;
                    return propertyValue.Cast<object>().Any();
                };
                return result;
            }
        }
    }
}
