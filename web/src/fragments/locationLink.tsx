import * as React from "react";
import { LinkProps } from "react-router-dom";

import { ILocation } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { PackageDoc } from "../util";
import { location as loc } from "./location";

export function locationLink(pkg: PackageDoc, location: ILocation, content: ReactFragment, linkProps?: LinkProps): ReactFragment {
    return loc(new FormatContext(pkg), location, content, linkProps);
}
