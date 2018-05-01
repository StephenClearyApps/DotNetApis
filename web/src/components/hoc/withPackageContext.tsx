import * as React from "react";

import { PackageRequestInjectedProps, ExtendingHoc, Hoc } from '.';
import { State } from "../../reducers";
import { PackageContext } from "../../util";

export type PackageContextInjectedProps = PackageContext;
export type PackageContextRequiredProps = State & PackageRequestInjectedProps;

function createWithPackageContext<TProps>(): Hoc<TProps & PackageContextRequiredProps, TProps & PackageContextRequiredProps & PackageContextInjectedProps> {
    return Component => props => {
        const packageStatus = props.packageDoc.packageDocumentation[props.pkgRequestStatus.normalizedPackageKey!];
        return <Component {...props} pkgStatus={packageStatus} pkg={packageStatus.json!}/>;
    };
}

/** Takes the package request and injects the package context (status and json) */
export const withPackageContext : ExtendingHoc<PackageContextInjectedProps, PackageContextRequiredProps> = createWithPackageContext();