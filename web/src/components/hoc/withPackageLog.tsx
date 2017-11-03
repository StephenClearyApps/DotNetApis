import * as React from "react";

import { PackageLoading } from "../PackageLoading";
import { PackageRequestError } from '../PackageRequestError';

import { withPackageContext, createLoadOnDemand, PackageRequestInjectedProps, createEither, withLoadPackageOnDemand, withPackageRequest, createRouterProps, PackageLogRequestInjectedProps, withPackageLogRequest, withLoadPackageLogOnDemand } from '.';
import { State } from "../../reducers/index";
import { Actions } from "../../actions";
import { PackageLogLoading } from "../PackageLogLoading";
import { PackageLogRequestError } from "../PackageLogRequestError";

export type PackageLogInjectedProps = PackageLogRequestInjectedProps;

export const withPackageLog =
<TProps extends {}>(Component: React.ComponentType<TProps & State & PackageLogRequestInjectedProps>) => {
    const withLoadingPackageMessage = createEither<TProps & State & PackageRequestInjectedProps>(
        props => props.pkgRequestStatus && props.pkgRequestStatus.status !== 'STARTED',
        props => <PackageLoading {...props}/>
    );
    const withLoadingPackageLogMessage = createEither<TProps & State & PackageLogRequestInjectedProps>(
        props => props.pkgLogRequestStatus && props.pkgLogRequestStatus.status !== 'STARTED',
        props => <PackageLogLoading {...props}/>
    );
    const withPackageErrorMessage = createEither<TProps & State & PackageRequestInjectedProps>(
        props => props.pkgRequestStatus.status !== 'ERROR',
        props => <PackageRequestError {...props}/>
    );
    const withPackageLogErrorMessage = createEither<TProps & State & PackageLogRequestInjectedProps>(
        props => props.pkgLogRequestStatus.status !== 'ERROR',
        props => <PackageLogRequestError {...props}/>
    );

    return createRouterProps<PackageKey>()(
        withPackageRequest(
            withLoadPackageOnDemand(
                withLoadingPackageMessage(
                    withPackageErrorMessage(
                        withPackageContext(
                            withPackageLogRequest(
                                withLoadPackageLogOnDemand(
                                    withLoadingPackageLogMessage(
                                        withPackageLogErrorMessage(
                                            Component))))))))));
}
