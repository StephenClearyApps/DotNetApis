import * as React from "react";

import { PackageRequestInjectedProps, ExtendingHoc } from '.';
import { State } from "../../reducers";
import { PackageContext } from "../../util";

export type PackageContextInjectedProps = PackageContext;

export function createPackageContext<TProps extends State & PackageRequestInjectedProps>(): ExtendingHoc<TProps, PackageContextInjectedProps> {
    return Component => props => {
        const packageStatus = props.packageDoc.packageDocumentation[props.pkgRequestStatus.normalizedPackageKey];
        return <Component {...props} pkgStatus={packageStatus} pkg={packageStatus.json}/>;
    };
}

export const withPackageContext =
<TProps extends {}>(Component: React.ComponentType<TProps & PackageContextInjectedProps>) =>
(props: TProps & State & PackageRequestInjectedProps) => {
    const packageStatus = props.packageDoc.packageDocumentation[props.pkgRequestStatus.normalizedPackageKey];
    return <Component {...props} pkgStatus={packageStatus} pkg={packageStatus.json}/>;
};
