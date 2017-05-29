using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO.Ably;

namespace Common.Internals
{
    public sealed class AblyService
    {
        private static readonly string AblyApiKey = Config.GetSetting("ABLY_API_KEY");

        private readonly AblyRealtime _client;

        private AblyService()
        {
            _client = new AblyRealtime(AblyApiKey);
        }

        public AblyChannel LogChannel(Guid operation)
        {
            return new AblyChannel(_client.Channels.Get("log:" + operation.ToString("N")), operation);
        }

        public static AblyService Instance { get; } = new AblyService();
    }
}
