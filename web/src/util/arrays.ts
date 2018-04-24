/** If the argument array is undefined or null, returns a new, empty array. Otherwise, returns its argument. */
export function array<T>(value?: T[]): T[] {
    return value || [];
}

export function flatten<T>(values?: T[][]): T[] {
    return array(values).reduce((a, b) => a.concat(b), []);
}

export function selectMany<TOuter, TInner>(values: TOuter[] | undefined, map: (value: TOuter) => TInner[]): TInner[] {
    return flatten(array(values).map(map));
}