import * as React from "react";
import { LinkProps } from "react-router-dom";

import { ILocation, isCurrentPackageLocation, isDependencyLocation } from "../../util/structure/locations";
import { PackageEntityLink } from "./PackageEntityLink";
import { ReferenceEntityLink } from "./ReferenceEntityLink";
import { PackageDoc } from "../../util/packageDoc";

interface LocationLinkProps {
    pkg: PackageDoc;
    includeLinks: boolean;
    location?: ILocation;
    linkProps?: LinkProps;
    children?: React.ReactNode;
}

export const LocationLink: React.StatelessComponent<LocationLinkProps> = ({ pkg, includeLinks, location, linkProps, children }) => {
    if (!location || !includeLinks)
        return <span>{children}</span>;
    else if (isCurrentPackageLocation(location))
        return <PackageEntityLink packageId={pkg.i} packageVersion={pkg.v} targetFramework={pkg.t} dnaid={location} linkProps={linkProps}>{children}</PackageEntityLink>;
    else if (isDependencyLocation(location))
        return <PackageEntityLink packageId={location.p} packageVersion={location.v} targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{children}</PackageEntityLink>;
    else
        return <ReferenceEntityLink targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{children}</ReferenceEntityLink>;
}
