using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Common
{
    public static class Constants
    {
        public static readonly UTF8Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        public static readonly SHA1Managed Sha1 = new SHA1Managed();

        public static readonly JsonSerializerSettings JsonSerializerSettings = CreateJsonSerializerSettings();

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var result = new JsonSerializerSettings
            {
                ContractResolver = new CustomContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
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
