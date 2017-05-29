using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO.Ably;
using IO.Ably.Rest;
using Microsoft.Azure.WebJobs.Host;

namespace Common
{
    /// <summary>
    /// A logger that writes messages both to Ably (if possible) and the <see cref="TextWriter"/> 
    /// </summary>
    public sealed class Logger
    {
        private static readonly string AblyApiKey = Config.GetSetting("ABLY_API_KEY");

        private readonly TraceWriter _writer;
        private readonly string _service;
        private readonly Guid _operation;
        private readonly IRestChannel _channel;

        public Logger(TraceWriter writer, string service, Guid operation)
        {
            _writer = writer;
            _service = service;
            _operation = operation;
            var client = new AblyRest(AblyApiKey);
            _channel = client.Channels.Get("log:" + operation.ToString("N"));
        }

        public void Trace(string message) => Write("trace", message);

        private void Write(string type, string message)
        {
            _writer?.Info(_service + ": " + message, _service); // TODO: fixup `message`.

            async Task PublishAsync()
            {
                try
                {
                    await _channel.PublishAsync(type, new
                    {
                        service = _service,
                        operation = _operation,
                        message,
                    });
                }
                catch (Exception e)
                {
                    _writer?.Info("Error publishing log: " + e.GetType().Name + ": " + e.Message, _service);
                }
            }

            var _ = PublishAsync();
        }
    }
}
