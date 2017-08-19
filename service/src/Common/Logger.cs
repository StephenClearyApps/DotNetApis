using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Internals;
using Microsoft.Azure.WebJobs.Host;

namespace Common
{
    public interface ILogger
    {
        void Trace(string message);
    }

    /// <summary>
    /// A logger that writes messages both to Ably (if possible) and the <see cref="TextWriter"/> 
    /// </summary>
    public sealed class Logger : ILogger
    {
        private readonly TraceWriter _writer;
        private readonly string _service;
        private readonly AblyChannel _channel;

        public Logger(TraceWriter writer, string service, Guid operation)
        {
            _writer = writer;
            _service = service;
            try
            {
                _channel = AblyService.Instance.LogChannel(operation);
            }
            catch (Exception ex)
            {
                _writer?.Warning($"Could not initialize Ably: [{ex.GetType().Name}]: {ex.Message}");
            }
        }

        public void Trace(string message) => Write("trace", message);

        private void Write(string type, string message)
        {
            _writer?.Info($"{_service}: {type}: {message}", _service);
            _channel?.LogMessage(_service, type, message);
        }
    }
}
