import * as React from "react";
import { Link, LinkProps } from "react-router-dom";
import { packageKey } from "../../util";

interface PackageLinkProps extends PackageKey {
    linkProps?: LinkProps;
}

export const PackageLink: React.StatelessComponent<PackageLinkProps> = ({ packageId, packageVersion, targetFramework, linkProps, children }) =>
    <Link {...linkProps} to={'/pkg/' + packageKey({ packageId, packageVersion, targetFramework })}>{children}</Link>;
