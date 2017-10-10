import { Location, History } from "history";
import { parse, stringify } from "query-string";

export function getHash<T extends object>(location: Location): T {
    return parse(location.hash);
}

export function replaceHash<T extends object>(history: History, value: T) {
    history.replace("#" + stringify(value));
}

export function pushHash<T extends object>(history: History, value: T) {
    history.push("#" + stringify(value));
}

interface FilterHash {
    filter: string;
}

export class HashFilter {
    constructor(location: Location, private history: History) {
        this.hash = getHash<FilterHash>(location);
    }

    hash: FilterHash;
    get filter(): string {
        return this.hash.filter || "";
    }
    set filter(value: string) {
        const replace = (this.hash.filter && value) || (!this.hash.filter && !value);
        this.hash.filter = value ? value : undefined;
        if (replace)
            replaceHash(this.history, this.hash);
        else
            pushHash(this.history, this.hash);
    }
}
