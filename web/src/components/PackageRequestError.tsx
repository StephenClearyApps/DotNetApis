import * as React from 'react';

import { ErrorDetails } from './ErrorDetails';

import { PackageRequestInjectedProps } from './hoc';
import { packageFriendlyName } from '../util';
import { State } from '../reducers';
import { PackageLogLink } from './links';

export interface PackageRequestErrorProps extends PackageRequestInjectedProps {
}

export const PackageRequestError: React.StatelessComponent<State & PackageRequestErrorProps> = (props) => {
    const { pkgRequestKey, pkgRequestStatus, time } = props;
    const error = (pkgRequestStatus && pkgRequestStatus.error) || null;
    const backendError = pkgRequestStatus && pkgRequestStatus.status === "BACKEND_ERROR";
    return (
        <div>
            <div style={{textAlign: "center"}}>
                <h1>Package Request Error</h1>
            </div>
            <p>There was an error when attempting to get the package documentation for {packageFriendlyName(pkgRequestKey)}.</p>
            <h2>Error details</h2>
            <ErrorDetails error={error} currentTimestamp={time.timestamp}/>
            {backendError ? <PackageLogLink {...pkgRequestKey}>View backend processing log</PackageLogLink> : null}
        </div>
    );
}
