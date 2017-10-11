import * as React from "react";
import { LinkProps } from "react-router-dom";

import { ILocation } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { PackageContext } from "../util";
import { location as loc } from "./location";

export function locationLink(pkgContext: PackageContext, location: ILocation, content: ReactFragment, linkProps?: LinkProps): ReactFragment {
    return loc(new FormatContext(pkgContext), location, content, linkProps);
}
