import { Location, History } from "history";
import { parse, stringify } from "query-string";

function getHash<T extends object>(location: Location): T {
    return parse(location.hash);
}

function replaceHash<T extends object>(history: History, value: T) {
    history.replace("#" + stringify(value));
}

function pushHash<T extends object>(history: History, value: T) {
    history.push("#" + stringify(value));
}

interface HashSettingsObject {
    [key: string]: string;
}

export class HashSettings {
    constructor(location: Location, private history: History, prefix: string) {
        this.hash = getHash<HashSettingsObject>(location);
        this.prefix = prefix ? prefix + "." : "";
    }

    private prefix: string;
    private hash: HashSettingsObject;

    getSetting(name: string): string {
        return this.hash[this.prefix + name] || "";
    }

    setSetting(name: string, value: string) {
        const key = this.prefix + name;
        const replace = (this.hash[key] && value) || (!this.hash[key] && !value);
        this.hash[key] = value ? value : undefined;
        if (replace)
            replaceHash(this.history, this.hash);
        else
            pushHash(this.history, this.hash);
    }
}
