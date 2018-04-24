import * as Ably from 'ably';

const client = new Ably.Realtime("uL8XyQ.FPHeQA:YpGFe6_ONYbEA4M_");

function attachAsync(channel: Ably.ablyLib.RealtimeChannel): Promise<void> {
    return new Promise((resolve, reject) => channel.attach(err => err ? reject(err) : resolve()));
}

export class LogListener {
    private channel: Ably.ablyLib.RealtimeChannel;
    constructor(name: string, private handler: (err: Ably.ablyLib.ErrorInfo | Error | undefined, message: Ably.ablyLib.Message | undefined, meta: string | undefined) => void) {
        this.channel = client.channels.get(name);
    }

    public async listen() {
        try {
            this.handler(undefined, undefined, "Establishing connection to log streaming service...");
            await attachAsync(this.channel);
            this.handler(undefined, undefined, "Connection established; subscribing to live updates...");
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
