import * as React from "react";

import { ReactComponent } from './util';
import { State } from "../../reducers";
import { PackageRequestInjectedProps } from "./PackageRequest";
import { PackageContext } from "../../util";

export type PackageInjectedProps = PackageContext;

export const withPackage =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageInjectedProps>) =>
    (props: TComponentProps & State & PackageRequestInjectedProps) => {
        const packageStatus = props.packageDoc.packageDocumentation[props.pkgRequestStatus.normalizedPackageKey];
        return <Component {...props} pkgStatus={packageStatus} pkg={packageStatus.json}/>;
    };
