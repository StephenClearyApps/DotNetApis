import * as React from "react";
import { Link, LinkProps } from "react-router-dom";
import { packageKey } from "../../util";

interface PackageLogLinkProps extends PackageKey {
    linkProps?: LinkProps;
}

export const PackageLogLink: React.StatelessComponent<PackageLogLinkProps> = ({ packageId, packageVersion, targetFramework, linkProps, children }) =>
    <Link {...linkProps} to={'/pkg/' + packageKey({ packageId, packageVersion, targetFramework }) + '/log'}>{children}</Link>;
