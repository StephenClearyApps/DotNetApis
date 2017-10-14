import * as React from "react";
import { Link, LinkProps } from "react-router-dom";
import { packageKey } from "../../util";

interface PackageFileLinkProps extends PackageKey {
    path: string;
    linkProps?: LinkProps;
}

export const PackageFileLink: React.StatelessComponent<PackageFileLinkProps> = ({ packageId, packageVersion, targetFramework, path, linkProps, children }) =>
    <Link {...linkProps} to={'/pkg/' + packageKey({ packageId, packageVersion, targetFramework }) + '/file/' + path}>{children}</Link>;
