import * as React from "react";
import { Link, LinkProps } from "react-router-dom";

interface ReferenceEntityLinkProps {
    targetFramework: string;
    dnaid: string;
    linkProps?: LinkProps;
}

export const ReferenceEntityLink: React.StatelessComponent<ReferenceEntityLinkProps> = ({ targetFramework, dnaid, linkProps, children }) =>
    <Link {...linkProps} to={'/ref/' + targetFramework + '/' + dnaid}>{children}</Link>;
