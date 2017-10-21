import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router';

import { ReactComponent } from './util';
import { State } from '../../reducers';
import { packageKey, PackageLogState } from '../../util';
import { PackageInjectedProps } from './Package';

export interface PackageLogRequestInjectedProps extends PackageInjectedProps {
    pkgLogRequestStatus: PackageLogState;
}

const packageLogRequest =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageLogRequestInjectedProps>) =>
    (props: TComponentProps & State & PackageInjectedProps) => {
        const request = props.packageLog.packageLogs[props.pkgRequestStatus.normalizedPackageKey];
        return <Component {...props} pkgLogRequestStatus={request}/>;
    };

/** Takes the package request parameters and injects `pkgLogRequestStatus` */
export const withPackageLogRequest =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageLogRequestInjectedProps>) =>
    packageLogRequest(Component);
