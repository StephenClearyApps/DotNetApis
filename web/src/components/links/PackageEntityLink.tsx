import * as React from "react";
import { Link, LinkProps } from "react-router-dom";

interface PackageEntityLinkProps {
    packageId: string;
    packageVersion: string;
    targetFramework: string;
    dnaid: string;
    linkProps?: LinkProps;
    children?: React.ReactNode;
}

export const PackageEntityLink: React.StatelessComponent<PackageEntityLinkProps> = ({ packageId, packageVersion, targetFramework, dnaid, linkProps, children }) =>
    <Link {...linkProps} to={'/pkg/' + packageId + '/' + packageVersion + '/' + targetFramework + '/doc/' + dnaid}>{children}</Link>;
