import * as React from "react";

import { ReactComponent, PackageRequestInjectedProps } from '.';
import { State } from "../../reducers";
import { PackageContext } from "../../util";

export type PackageContextInjectedProps = PackageContext;

export const withPackageContext =
    <TProps extends {}>(Component: ReactComponent<TProps & PackageContextInjectedProps>) =>
    (props: TProps & State & PackageRequestInjectedProps) => {
        const packageStatus = props.packageDoc.packageDocumentation[props.pkgRequestStatus.normalizedPackageKey];
        return <Component {...props} pkgStatus={packageStatus} pkg={packageStatus.json}/>;
    };
