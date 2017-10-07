import * as React from "react";
import { Link, LinkProps } from "react-router-dom";

interface ReferenceEntityLinkProps {
    targetFramework: string;
    dnaid: string;
    linkProps?: LinkProps;
    children?: React.ReactNode;
}

export const ReferenceEntityLink = ({ targetFramework, dnaid, linkProps, children }: ReferenceEntityLinkProps) =>
    <Link {...linkProps} to={'/ref/' + targetFramework + '/' + dnaid}>{children}</Link>;
