using System;
using IO.Ably;

namespace DotNetApis.Common.Internals
{
    public static class AblyService
    {
        // "Singleton" is not the right word for this. But "RetryableFactory" is just too enterprisey.
        private static readonly ISingleton<AblyRealtime> Client = Singleton.Create(() => new AblyRealtime(Config.GetSetting("ABLY_API_KEY")));

        public static AblyChannel CreateLogChannel(string name) => new AblyChannel(Client.Value.Channels.Get("log:" + name));
    }
}
