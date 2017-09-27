using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Common.LogStructure
{
    public sealed class LogMessage
    {
        public LogLevel Type { get; set; }

        [JsonConverter(typeof(EpochTimestampJsonConverter))]
        public DateTimeOffset Timestamp { get; set; }

        public string Message { get; set; }
    }
}
