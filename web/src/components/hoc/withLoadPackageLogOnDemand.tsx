import { createLoadOnDemand, PackageLogRequestInjectedProps, Hoc } from '.';
import { State } from "../../reducers";
import { Actions } from "../../actions";

type LoadPackageLogOnDemandRequiredProps = State & Actions & PackageLogRequestInjectedProps;

function createLoadPackageLogOnDemand<TProps>(): Hoc<{}, LoadPackageLogOnDemandRequiredProps> {
    return createLoadOnDemand<LoadPackageLogOnDemandRequiredProps>({
        hasStarted: props => !!props.pkgLogRequestStatus,
        load: props => props.PackageLogActions.getLog(props.pkgRequestStatus.normalizedPackageKey!, props.pkgStatus.logUri!)
    }) as any;
}

/** Takes the package log request props and loads the package log */
export const withLoadPackageLogOnDemand = createLoadPackageLogOnDemand();
