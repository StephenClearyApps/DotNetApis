import * as React from 'react';

import { LogMessages } from './LogMessages';

import { PackageLogRequestInjectedProps, withPackageLog } from './hoc';
import { packageFriendlyName } from '../util';
import { State } from '../reducers';

export interface PackageLogProps extends PackageLogRequestInjectedProps {
}

const PackageLogComponent: React.StatelessComponent<State & PackageLogProps> = (props) => {
    const { pkgLogRequestStatus, pkgRequestStatus, time } = props;
    const log = (pkgLogRequestStatus && pkgLogRequestStatus.log) || null;
    return (
        <div>
            <div style={{textAlign: "center"}}>
                <h1>Processing Log for {pkgRequestStatus.normalizedPackageKey}</h1>
            </div>
            <LogMessages messages={log} currentTimestamp={time.timestamp}/>
        </div>
    );
}

export const PackageLog = withPackageLog(PackageLogComponent);