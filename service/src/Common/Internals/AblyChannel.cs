using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO.Ably.Realtime;

namespace Common.Internals
{
    public sealed class AblyChannel
    {
        private readonly IRealtimeChannel _channel;
        private readonly Guid _operation;

        public AblyChannel(IRealtimeChannel channel, Guid operation)
        {
            _channel = channel;
            _operation = operation;
        }

        public void LogMessage(string service, string type, string message)
        {
            _channel.PublishAsync(type, new
            {
                message,
                operation = _operation.ToString("N"),
                service,
            });
        }
    }
}
