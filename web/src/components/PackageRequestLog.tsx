import * as React from 'react';

import { LogMessages } from './LogMessages';

import { PackageRequestInjectedProps } from './hoc';
import { State } from '../reducers';

export interface PackageRequestLogProps extends PackageRequestInjectedProps {
}

export const PackageRequestLog: React.StatelessComponent<State & PackageRequestLogProps> = ({ pkgRequestStatus, time }) => {
    const requestMessages = pkgRequestStatus && pkgRequestStatus.log ?
        <LogMessages currentTimestamp={time.timestamp} messages={pkgRequestStatus.log} /> : null;
    const streamingMessages = pkgRequestStatus && pkgRequestStatus.streamingLog ?
        <LogMessages currentTimestamp={time.timestamp} messages={pkgRequestStatus.streamingLog} /> : null;
    if (!requestMessages && !streamingMessages)
        return null;
    return (
        <div>
            {requestMessages}
            {streamingMessages}
        </div>
    );
}
