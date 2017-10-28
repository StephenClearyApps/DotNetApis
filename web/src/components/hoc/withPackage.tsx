import * as React from "react";

import { PackageLoading } from "../PackageLoading";
import { PackageRequestError } from '../PackageRequestError';

import { PackageContextInjectedProps, ReactComponent, withPackageContext, createLoadOnDemand, PackageRequestInjectedProps, createEither, withLoadPackageOnDemand, withPackageRequest, createRouterProps } from '.';
import { State } from "../../reducers/index";
import { Actions } from "../../actions";

export type PackageInjectedProps = PackageContextInjectedProps;

export const withPackage =
<TProps extends {}>(Component: ReactComponent<TProps & PackageInjectedProps>) => {
    const withEitherLoadingMessage = createEither<TProps & State & PackageRequestInjectedProps>(
        props => props.pkgRequestStatus && props.pkgRequestStatus.status !== 'STARTED',
        props => <PackageLoading {...props}/>
    );
    const withEitherErrorMessage = createEither<TProps & State & PackageRequestInjectedProps>(
        props => props.pkgRequestStatus.status !== 'ERROR' && props.pkgRequestStatus.status !== 'BACKEND_ERROR',
        props => <PackageRequestError {...props}/>
    );

    return createRouterProps<PackageKey>()(
        withPackageRequest(
            withLoadPackageOnDemand(
                withEitherLoadingMessage(
                    withEitherErrorMessage(
                        withPackageContext(Component))))));
}
