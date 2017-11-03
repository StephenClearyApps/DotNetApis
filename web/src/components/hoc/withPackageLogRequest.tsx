import * as React from 'react';

import { RouteComponentProps, PackageInjectedProps } from '.';
import { State } from '../../reducers';
import { packageKey, PackageLogState } from '../../util';

export interface PackageLogRequestInjectedProps extends PackageInjectedProps {
    pkgLogRequestStatus: PackageLogState;
}

/** Takes the package request parameters and injects `PackageLogRequestInjectedProps` */
export const withPackageLogRequest =
    <TProps extends {}>(Component: React.ComponentType<TProps & PackageLogRequestInjectedProps>) =>
    (props: TProps & State & PackageInjectedProps) => {
        const request = props.packageLog.packageLogs[props.pkgRequestStatus.normalizedPackageKey];
        return <Component {...props} pkgLogRequestStatus={request}/>;
    };
