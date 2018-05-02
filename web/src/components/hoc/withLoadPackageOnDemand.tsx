import { createLoadOnDemand, PackageRequestInjectedProps, Hoc } from '.';
import { State } from "../../reducers";
import { Actions } from "../../actions";

type LoadPackageOnDemandRequiredProps = State & Actions & PackageRequestInjectedProps;

function createLoadPackageOnDemand<TProps>(): Hoc<{}, LoadPackageOnDemandRequiredProps> {
    return createLoadOnDemand<LoadPackageOnDemandRequiredProps>({
        hasStarted: props => !!props.pkgRequestStatus,
        load: props => props.PackageDocActions.getDoc(props.pkgRequestKey)
    }) as any;
}

/** Takes the package request props and loads the package */
export const withLoadPackageOnDemand = createLoadPackageOnDemand();