export * from "../../util/formatContext";
export { array } from "../../util";

import { ILocation, isCurrentPackageLocation } from "../../structure";

// Note that react elements are not react fragments; returning bare elements prevents React from auto-assigning keys, so they must be wrapped in an array.
interface ReactFragmentArray extends Array<ReactFragment> { }
interface ReactElementArray extends Array<React.ReactElement<any>> { }
export type ReactFragment = string | null | ReactFragmentArray | ReactElementArray;

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
