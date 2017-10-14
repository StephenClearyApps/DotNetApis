import * as React from "react";
import { Link, LinkProps } from "react-router-dom";
import { packageKey } from "../../util";

interface PackageNamespaceLinkProps extends PackageKey {
    ns: string;
    linkProps?: LinkProps;
}

export const PackageNamespaceLink: React.StatelessComponent<PackageNamespaceLinkProps> = ({ packageId, packageVersion, targetFramework, ns, linkProps, children }) =>
    <Link {...linkProps} to={'/pkg/' + packageKey({ packageId, packageVersion, targetFramework }) + '/ns/' + ns}>{children}</Link>;
