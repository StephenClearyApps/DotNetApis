import * as React from 'react';

import { ErrorDetails } from './ErrorDetails';

import { PackageLogRequestInjectedProps } from './hoc';
import { packageFriendlyName } from '../util';
import { State } from '../reducers';

export interface PackageLogRequestErrorProps extends PackageLogRequestInjectedProps {
}

export const PackageLogRequestError: React.StatelessComponent<State & PackageLogRequestInjectedProps> = (props) => {
    const { pkgLogRequestStatus, pkgRequestStatus, time } = props;
    const error = (pkgLogRequestStatus && pkgLogRequestStatus.error) || undefined;
    return (
        <div>
            <div style={{textAlign: "center"}}>
                <h1>Package Log Request Error</h1>
            </div>
            <p>There was an error when attempting to get the processing log for {pkgRequestStatus.normalizedPackageKey}.</p>
            <h2>Error details</h2>
            <ErrorDetails error={error} currentTimestamp={time.timestamp}/>
        </div>
    );
}
