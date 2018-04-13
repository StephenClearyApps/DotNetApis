import * as React from "react";

import { PackageLoading } from "../PackageLoading";
import { PackageRequestError } from '../PackageRequestError';

import { PackageContextInjectedProps, createPackageContext, createLoadOnDemand, PackageRequestInjectedProps, createEither, withLoadPackageOnDemand, withPackageRequest, createRouterProps, createLoadPackageOnDemand } from '.';
import { State } from "../../reducers/index";
import { Actions } from "../../actions";

export type PackageInjectedProps = PackageContextInjectedProps;

export const withPackage =
<TProps extends State & Actions & PackageInjectedProps>(Component: React.ComponentType<TProps>) => {
    const withEitherLoadingMessage = createEither<TProps & PackageRequestInjectedProps>(
        props => props.pkgRequestStatus && props.pkgRequestStatus.status !== 'STARTED',
        props => <PackageLoading {...props}/>
    );
    const withEitherErrorMessage = createEither<TProps & PackageRequestInjectedProps>(
        props => props.pkgRequestStatus.status !== 'ERROR' && props.pkgRequestStatus.status !== 'BACKEND_ERROR',
        props => <PackageRequestError {...props}/>
    );
    const withPackageContext = createPackageContext<TProps>();
    const withLoadPackageOnDemand = createLoadPackageOnDemand<TProps>();
    var a = Component;
    var b = withPackageContext(a);
    var c = withEitherErrorMessage(b);

    var x = withEitherLoadingMessage(withEitherErrorMessage(withPackageContext(Component)));
    var y = withLoadPackageOnDemand(x);

    return createRouterProps<PackageKey, TProps>()(
        withPackageRequest(
            withLoadPackageOnDemand(
                withEitherLoadingMessage(
                    withEitherErrorMessage(
                        withPackageContext(Component))))));
}
