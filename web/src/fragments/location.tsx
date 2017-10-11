import * as React from "react";
import { LinkProps } from "react-router-dom";

import { ILocation, isCurrentPackageLocation, isDependencyLocation } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { PackageEntityLink, ReferenceEntityLink } from "../components/links";

export function location(context: FormatContext, location: ILocation, content: ReactFragment, linkProps?: LinkProps): ReactFragment {
    const { pkg } = context;
    if (!location || !context.includeLinks)
        return content;
    const children = React.Children.toArray(content);
    if (isCurrentPackageLocation(location))
        return [<PackageEntityLink {...context.pkgContext.pkgRequestKey} dnaid={location} linkProps={linkProps}>{children}</PackageEntityLink>];
    else if (isDependencyLocation(location))
        return [<PackageEntityLink packageId={location.p} packageVersion={location.v} targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{children}</PackageEntityLink>];
    else
        return [<ReferenceEntityLink targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{children}</ReferenceEntityLink>];
}
