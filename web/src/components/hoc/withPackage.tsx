import * as React from "react";

import { Hoc, ExtendingHoc } from ".";
import { PackageLoading } from "../PackageLoading";
import { PackageRequestError } from '../PackageRequestError';

import { PackageContextInjectedProps, withPackageContext, PackageRequestInjectedProps, createEither, withLoadPackageOnDemand, withPackageRequest, createRouterProps } from '.';
import { State } from "../../reducers";

export type PackageInjectedProps = PackageContextInjectedProps;

function createWithPackage<TProps>(): Hoc<TProps, TProps & PackageInjectedProps> {
    return Component => {
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
    };
}

/** Determines the current package from the router, and injects the package context */
export const withPackage : ExtendingHoc<PackageInjectedProps> = createWithPackage();
