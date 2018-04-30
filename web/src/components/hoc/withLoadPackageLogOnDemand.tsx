import { createLoadOnDemand, PackageLogRequestInjectedProps, Hoc, PassthroughHoc } from '.';
import { State } from "../../reducers";
import { Actions } from "../../actions";

type LoadPackageLogOnDemandRequiredProps = State & Actions & PackageLogRequestInjectedProps;

function createLoadPackageLogOnDemand<TProps>(): Hoc<TProps & LoadPackageLogOnDemandRequiredProps> {
    return createLoadOnDemand<TProps & LoadPackageLogOnDemandRequiredProps>({
        hasStarted: props => !!props.pkgLogRequestStatus,
        load: props => props.PackageLogActions.getLog(props.pkgRequestStatus.normalizedPackageKey!, props.pkgStatus.logUri!)
    });
}

/** Takes the package log request props and loads the package log */
export const withLoadPackageLogOnDemand : PassthroughHoc<LoadPackageLogOnDemandRequiredProps> = createLoadPackageLogOnDemand();