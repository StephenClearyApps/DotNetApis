import * as React from 'react';
import { CircularProgress } from 'material-ui';

import { State } from "../reducers";
import { PackageRequestInjectedProps } from "./hoc";
import { LogMessages } from './LogMessages';

export type PackageLoadingProps = State & PackageRequestInjectedProps;

export const PackageLoading: React.StatelessComponent<PackageLoadingProps> = ({ time, pkgRequestStatus, pkgRequestKey }) => {
    const requestMessages = pkgRequestStatus && pkgRequestStatus.log ?
        <LogMessages currentTimestamp={time.timestamp} messages={pkgRequestStatus.log} /> : null;
    const streamingMessages = pkgRequestStatus && pkgRequestStatus.streamingLog ?
        <LogMessages currentTimestamp={time.timestamp} messages={pkgRequestStatus.streamingLog} /> : null;
    return (
        <div style={{textAlign: "center"}}>
            <p>{"Loading documentation for " + pkgRequestKey.packageId}</p>
            {requestMessages}
            {streamingMessages}
            <div style={{marginTop: "1em", marginBottom: "1em"}}><CircularProgress/></div>
        </div>
    );
}