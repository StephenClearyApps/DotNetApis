import * as React from "react";

import { createLoadOnDemand, PackageRequestInjectedProps, Hoc } from '.';
import { State } from "../../reducers/index";
import { Actions } from "../../actions";

export function createLoadPackageOnDemand<TProps extends Actions & PackageRequestInjectedProps>(): Hoc<TProps> {
    return createLoadOnDemand({
        hasStarted: props => !!props.pkgRequestStatus,
        load: props => props.DocActions.getDoc(props.pkgRequestKey)
    });
}

export function test<TProps extends Actions & PackageRequestInjectedProps>(Component: React.ComponentType<TProps>) {
    return createLoadPackageOnDemand<TProps>()(Component);
}

/** Takes the package request props and loads the package */
export const withLoadPackageOnDemand =
<TProps extends {}>(Component: React.ComponentType<TProps & State & Actions & PackageRequestInjectedProps>) =>
createLoadOnDemand<TProps & State & Actions & PackageRequestInjectedProps>({
    hasStarted: props => !!props.pkgRequestStatus,
    load: props => props.DocActions.getDoc(props.pkgRequestKey)
})(Component);
