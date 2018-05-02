import * as React from "react";

import { Hoc } from ".";
import { PackageLoading } from "../PackageLoading";
import { PackageRequestError } from '../PackageRequestError';

import { PackageContextInjectedProps, withPackageContext, PackageRequestInjectedProps, createEither, withLoadPackageOnDemand, withPackageRequest, createRouterProps } from '.';
import { State } from "../../reducers";

export type PackageInjectedProps = PackageContextInjectedProps;

function createWithPackage<TProps>(): Hoc<PackageInjectedProps> {
    return Component => {
        const withEitherLoadingMessage = createEither<State & PackageRequestInjectedProps>(
            props => props.pkgRequestStatus && props.pkgRequestStatus.status !== 'STARTED',
            props => <PackageLoading {...props}/>
        );
        const withEitherErrorMessage = createEither<State & PackageRequestInjectedProps>(
            props => props.pkgRequestStatus.status !== 'ERROR' && props.pkgRequestStatus.status !== 'BACKEND_ERROR',
            props => <PackageRequestError {...props}/>
        );

        // Get PackageKey from URI router parameters.
        const result = createRouterProps<PackageKey>()(
            // Construct the request for the package.
            withPackageRequest(
                // Load the package if it is not already loaded.
                withLoadPackageOnDemand(
                    // Show loading message while the package is loading.
                    withEitherLoadingMessage(
                        // Show error message if the package load failed.
                        withEitherErrorMessage(
                            // Load the package data.
                            withPackageContext(Component))))));
        return result as any;
    };
}

/** Determines the current package from the router, and injects the package context */
export const withPackage = createWithPackage();
