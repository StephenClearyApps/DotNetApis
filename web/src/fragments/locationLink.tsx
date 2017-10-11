import * as React from "react";
import { LinkProps } from "react-router-dom";

import { ILocation } from "../structure";
import { PackageContext, FormatContext } from "../util";
import { location as partialLocation, ReactFragment } from "./partial";

export function locationLink(pkgContext: PackageContext, location: ILocation, content: ReactFragment, linkProps?: LinkProps): React.ReactChild[] {
    const context = new FormatContext(pkgContext);
    return React.Children.toArray(partialLocation(context, location, content, linkProps));
}
