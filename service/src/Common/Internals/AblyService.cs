using System;
using IO.Ably;
using Config = DotNetApis.Common.Config;

namespace DotNetApis.Common.Internals
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
