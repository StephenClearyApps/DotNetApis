import * as React from 'react';

import { PackageInjectedProps, Hoc } from '.';
import { State } from '../../reducers';
import { PackageLogState } from '../../ducks/packageLog';

// TODO: Consider removing PackageInjectedProps
export interface PackageLogRequestInjectedProps extends PackageInjectedProps {
    pkgLogRequestStatus: PackageLogState;
}
export type PackageLogRequestRequiredProps = State & PackageInjectedProps;

function createWithPackageLogRequest<TProps>(): Hoc<PackageLogRequestInjectedProps, PackageLogRequestRequiredProps> {
    return Component => props => {
        const request = props.packageLog.packageLogs[props.pkgRequestStatus.normalizedPackageKey!];
        let TComponent: any = Component; // we pass extra props
        return <TComponent {...props} pkgLogRequestStatus={request}/>;
    };
}

/** Takes the package request parameters and injects `PackageLogRequestInjectedProps` */
export const withPackageLogRequest = createWithPackageLogRequest();
