using System;
using IO.Ably.Realtime;

namespace DotNetApis.Common.Internals
{
    public sealed class AblyChannel
    {
        private readonly IRealtimeChannel _channel;

        public AblyChannel(IRealtimeChannel channel)
        {
            _channel = channel;
        }

        public void LogMessage(string type, string message)
        {
            _channel.PublishAsync(type, new
            {
                message,
            });
        }
    }
}
