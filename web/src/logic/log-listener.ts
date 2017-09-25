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

export class LogListener {
    private channel: Ably.ablyLib.RealtimeChannel;
    constructor(name: string, private handler: (err: Ably.ablyLib.ErrorInfo | Error, message: Ably.ablyLib.Message, meta: string) => void) {
        this.channel = client.channels.get(name);
    }

    public async listen() {
        try {
            this.handler(undefined, undefined, "Establishing connection to log streaming service...");
            const buffer: Ably.ablyLib.Message[] = [];
            await attachAsync(this.channel);
            this.handler(undefined, undefined, "Connection established; reviewing history...");
            let page = await historyAsync(this.channel, { untilAttach: true, direction: "forwards" });
            while (true) {
                buffer.push(...page.items);
                if (page.isLast()) {
                    break;
                }
                page = await nextAsync(page);
            }
            this.handler(undefined, undefined, "Switching to live updates...");
            for (let message of buffer) {
                this.handler(undefined, message, undefined);
            }
            this.channel.subscribe(message => {
                this.handler(undefined, message, undefined);
            });
        } catch (e) {
            this.handler(e, undefined, undefined);
            this.dispose();
        }
    }

    public dispose() {
        this.channel.unsubscribe();
        this.channel.detach();
    }
}
