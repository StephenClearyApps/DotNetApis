import * as Ably from 'ably';

const client = new Ably.Realtime("uL8XyQ.FPHeQA:YpGFe6_ONYbEA4M_");

function attachAsync(channel: Ably.ablyLib.RealtimeChannel): Promise<void> {
    return new Promise((resolve, reject) => channel.attach(err => err ? reject(err) : resolve()));
}

function historyAsync(channel: Ably.ablyLib.RealtimeChannel, params: Ably.ablyLib.RealtimePresenceHistoryParams): Promise<Ably.ablyLib.PaginatedResult<Ably.ablyLib.Message>> {
    return new Promise((resolve, reject) => channel.history(params, (err, page) => err ? reject(err) : resolve(page)));
}

function nextAsync<T>(page: Ably.ablyLib.PaginatedResult<T>): Promise<Ably.ablyLib.PaginatedResult<T>> {
    return new Promise((resolve, reject) => page.next((err, nextPage) => err ? reject(err) : resolve(nextPage)));
}

export async function listen(channelName: string, handler: (err: Ably.ablyLib.ErrorInfo, message: Ably.ablyLib.Message, meta: string) => void) {
    handler(undefined, undefined, "Establishing connection to log streaming service...");
    const buffer: Ably.ablyLib.Message[] = [];
    const channel = client.channels.get(channelName);
    try {
        await attachAsync(channel);
        handler(undefined, undefined, "Connection established; reviewing history...");
        let page = await historyAsync(channel, { untilAttach: true, direction: "forwards" });
        while (true) {
            buffer.push(...page.items);
            if (page.isLast()) {
                break;
            }
            page = await nextAsync(page);
        }
        handler(undefined, undefined, "Switching to live updates...");
        for (let message of buffer) {
            handler(undefined, message, undefined);
        }
        channel.subscribe(message => {
            handler(undefined, message, undefined);
        });
    } catch (e) {
        channel.unsubscribe(); // TODO: provide a way for callers to unsubscribe.
        channel.detach();
        handler(e, undefined, undefined);
    }
}
