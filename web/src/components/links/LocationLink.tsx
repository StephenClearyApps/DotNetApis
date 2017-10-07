import * as React from "react";
import { LinkProps } from "react-router-dom";

import { ILocation, isCurrentPackageLocation, isDependencyLocation, FormatContext } from "../../util";
import { PackageEntityLink } from "./PackageEntityLink";
import { ReferenceEntityLink } from "./ReferenceEntityLink";

interface LocationLinkProps {
    context: FormatContext;
    location?: ILocation;
    linkProps?: LinkProps;
}

export const LocationLink: React.StatelessComponent<LocationLinkProps> = ({ context, location, linkProps, children }) => {
    const { pkg } = context;
    if (!location || !context.includeLinks)
        return <span>{children}</span>;
    else if (isCurrentPackageLocation(location))
        return <PackageEntityLink packageId={pkg.i} packageVersion={pkg.v} targetFramework={pkg.t} dnaid={location} linkProps={linkProps}>{children}</PackageEntityLink>;
    else if (isDependencyLocation(location))
        return <PackageEntityLink packageId={location.p} packageVersion={location.v} targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{children}</PackageEntityLink>;
    else
        return <ReferenceEntityLink targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{children}</ReferenceEntityLink>;
}
