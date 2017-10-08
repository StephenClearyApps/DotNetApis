export * from "../util/formatContext";

import { ILocation, isCurrentPackageLocation } from "../structure";

// https://github.com/Microsoft/TypeScript/issues/3496#issuecomment-128553540
interface ReactFragmentArray extends Array<ReactFragment> { }
export type ReactFragment = string | null | React.ReactElement<any> | ReactFragmentArray;

export function join(components: ReactFragment[], separator: ReactFragment): ReactFragment {
    const last = components.length - 1;
    return components.map<ReactFragment[] | ReactFragment>((x, i) => i === last ? x : [x, separator]);
}

export function locationDnaid(location: ILocation): string {
    if (!location)
        return '';
    else if (isCurrentPackageLocation(location))
        return location;
    else
        return location.i;
}
