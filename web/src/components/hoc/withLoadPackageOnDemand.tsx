import * as React from "react";

import { createLoadOnDemand, PackageRequestInjectedProps } from '.';
import { State } from "../../reducers/index";
import { Actions } from "../../actions";

/** Takes the package request props and loads the package */
export const withLoadPackageOnDemand =
<TProps extends {}>(Component: React.ComponentType<TProps & State & Actions & PackageRequestInjectedProps>) =>
createLoadOnDemand<TProps & State & Actions & PackageRequestInjectedProps>({
    hasStarted: props => !!props.pkgRequestStatus,
    load: props => props.DocActions.getDoc(props.pkgRequestKey)
})(Component);
