import * as React from 'react';
import { CircularProgress } from 'material-ui';

import { State } from "../reducers";
import { PackageRequestInjectedProps } from "./hoc";
import { LogMessages } from './LogMessages';

export type PackageLoadingProps = State & PackageRequestInjectedProps;

export const PackageLoading: React.StatelessComponent<PackageLoadingProps> = ({ time, request, requestParams }) => {
    const requestMessages = request && request.log ?
        <LogMessages currentTimestamp={time.timestamp} messages={request.log} /> : null;
    const streamingMessages = request && request.streamingLog ?
        <LogMessages currentTimestamp={time.timestamp} messages={request.streamingLog} /> : null;
    return (
        <div style={{textAlign: "center"}}>
            <p>{"Loading documentation for " + requestParams.packageId}</p>
            {requestMessages}
            {streamingMessages}
            <div style={{marginTop: "1em", marginBottom: "1em"}}><CircularProgress/></div>
        </div>
    );
}