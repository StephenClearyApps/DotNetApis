import * as React from "react";

import { PackageLoading } from "../PackageLoading";
import { PackageRequestError } from '../PackageRequestError';

import { Hoc } from ".";
import { withPackageContext, PackageRequestInjectedProps, createEither, withLoadPackageOnDemand, withPackageRequest, createRouterProps, PackageLogRequestInjectedProps, withPackageLogRequest, withLoadPackageLogOnDemand } from '.';
import { State } from "../../reducers";
import { PackageLogLoading } from "../PackageLogLoading";
import { PackageLogRequestError } from "../PackageLogRequestError";

export type PackageLogInjectedProps = PackageLogRequestInjectedProps;
export type PackageLogRequiredProps = State;

function createWithPackageLog<TProps>(): Hoc<PackageLogInjectedProps, PackageLogRequiredProps> {
    return Component => {
        const withLoadingPackageMessage = createEither<State & PackageRequestInjectedProps>(
            props => props.pkgRequestStatus && props.pkgRequestStatus.status !== 'STARTED',
            props => <PackageLoading {...props}/>
        );
        const withLoadingPackageLogMessage = createEither<State & PackageLogRequestInjectedProps>(
            props => props.pkgLogRequestStatus && props.pkgLogRequestStatus.status !== 'STARTED',
            props => <PackageLogLoading {...props}/>
        );
        const withPackageErrorMessage = createEither<State & PackageRequestInjectedProps>(
            props => props.pkgRequestStatus.status !== 'ERROR',
            props => <PackageRequestError {...props}/>
        );
        const withPackageLogErrorMessage = createEither<State & PackageLogRequestInjectedProps>(
            props => props.pkgLogRequestStatus.status !== 'ERROR',
            props => <PackageLogRequestError {...props}/>
        );

        // Get PackageKey from URI router parameters.
        const result = createRouterProps<PackageKey>()(
            // Construct the request for the package.
            withPackageRequest(
                // Load the package if it is not already loaded.
                withLoadPackageOnDemand(
                    // Show loading message while the package is loading.
                    withLoadingPackageMessage(
                        // Show error message if the package load failed.
                        withPackageErrorMessage(
                            // Load the package data.
                            withPackageContext(
                                // Construct the request for the log.
                                withPackageLogRequest(
                                    // Load the package log if it is not already loaded.
                                    withLoadPackageLogOnDemand(
                                        // Show loading message while the log is loading.
                                        withLoadingPackageLogMessage(
                                            // Show error message if the log load failed.
                                            withPackageLogErrorMessage(
                                                Component))))))))));
        return result as any;
    };
}

/** Determines the current package from the router, and injects the package log */
export const withPackageLog = createWithPackageLog();
