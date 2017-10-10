import * as React from "react";
import { Link, LinkProps } from "react-router-dom";
import { packageKey } from "../../util";

interface PackageEntityLinkProps extends PackageKey {
    dnaid: string;
    linkProps?: LinkProps;
}

export const PackageEntityLink: React.StatelessComponent<PackageEntityLinkProps> = ({ packageId, packageVersion, targetFramework, dnaid, linkProps, children }) =>
    <Link {...linkProps} to={'/pkg/' + packageKey({ packageId, packageVersion, targetFramework }) + '/doc/' + dnaid}>{children}</Link>;
