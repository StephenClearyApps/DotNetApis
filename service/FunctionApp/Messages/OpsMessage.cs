using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FunctionApp.Messages
{
    public enum OpsMessageType
    {
        ProcessReferenceXmldoc = 1,
    }

    public class OpsMessage
    {
        public OpsMessageType Type { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> Arguments { get; set; }
    }
}
