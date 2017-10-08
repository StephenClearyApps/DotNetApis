export * from "../util/formatContext";

import { ILocation, isCurrentPackageLocation } from "../structure";

// https://github.com/Microsoft/TypeScript/issues/3496#issuecomment-128553540
interface ReactFragmentArray extends Array<ReactFragment> { }
export type ReactFragment = string | null | React.ReactElement<any> | ReactFragmentArray;

export function join(fragments: ReactFragment[], separator: ReactFragment): ReactFragment {
    const last = fragments.length - 1;
    return fragments.map<ReactFragment>((x, i) => i === last ? x : [x, separator]);
}

export function locationDnaid(location: ILocation): string {
    if (!location)
        return '';
    else if (isCurrentPackageLocation(location))
        return location;
    else
        return location.i;
}

/** If the argument array is undefined or null, returns a new, empty array. Otherwise, returns its argument. */
export function array<T>(value: T[]): T[] {
    return value || [];
}