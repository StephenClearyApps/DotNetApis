import { createLoadOnDemand, PackageRequestInjectedProps, PassthroughHoc, Hoc } from '.';
import { State } from "../../reducers";
import { Actions } from "../../actions";

type LoadPackageOnDemandRequiredProps = State & Actions & PackageRequestInjectedProps;

function createLoadPackageOnDemand<TProps>(): Hoc<TProps & LoadPackageOnDemandRequiredProps> {
    return createLoadOnDemand<TProps & LoadPackageOnDemandRequiredProps>({
        hasStarted: props => !!props.pkgRequestStatus,
        load: props => props.PackageDocActions.getDoc(props.pkgRequestKey)
    });
}

/** Takes the package request props and loads the package */
export const withLoadPackageOnDemand : PassthroughHoc<LoadPackageOnDemandRequiredProps> = createLoadPackageOnDemand();