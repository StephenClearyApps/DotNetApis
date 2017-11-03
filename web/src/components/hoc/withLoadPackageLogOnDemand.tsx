import * as React from "react";

import { createLoadOnDemand, PackageLogRequestInjectedProps } from '.';
import { State } from "../../reducers/index";
import { Actions } from "../../actions";

/** Takes the package request props and loads the package */
export const withLoadPackageLogOnDemand =
<TProps extends {}>(Component: React.ComponentType<TProps & State & Actions & PackageLogRequestInjectedProps>) =>
createLoadOnDemand<TProps & State & Actions & PackageLogRequestInjectedProps>({
    hasStarted: props => !!props.pkgLogRequestStatus,
    load: props => props.DocActions.getDocLog(props.pkgRequestStatus.normalizedPackageKey, props.pkgStatus.logUri)
})(Component);
