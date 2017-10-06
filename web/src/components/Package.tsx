import * as React from "react";
import { match, RouteComponentProps } from "react-router";
import { CircularProgress } from 'material-ui';

import { LogMessages } from "./LogMessages";
import { State } from "../reducers";
import { Actions } from "../actions";
import { withLoadOnDemand } from "./hoc";
import { packageKey } from "../util/packageKey";

interface RouteParams {
    packageId: string;
    packageVersion?: string;
    targetFramework?: string;
}

function PackageComponent(props: State & Actions & RouteComponentProps<RouteParams>) {
    const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
    if (request.status === 'ERROR')
        return <div>TODO: error display</div>;
    const doc = props.packageDoc.packageDocumentation[request.normalizedPackageKey];
    return (
    <div>
        {JSON.stringify(doc)}
    </div>);
}

function LoadingPackageComponent(props: State & Actions & RouteComponentProps<RouteParams>) {
    const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
    const requestMessages = request && request.log ? <LogMessages currentTimestamp={props.time.timestamp} messages={request.log} /> : null;
    const streamingMessages = request && request.streamingLog ? <LogMessages currentTimestamp={props.time.timestamp} messages={request.streamingLog} /> : null;
    return (
        <div style={{textAlign: "center"}}>
            <p>{"Loading documentation for " + props.match.params.packageId}</p>
            {requestMessages}
            {streamingMessages}
            <p><CircularProgress/></p>
        </div>
    );
}

export const Package = withLoadOnDemand<State & Actions & RouteComponentProps<RouteParams>>({
    hasStarted: props => {
        const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
        return !!request;
    },
    isLoaded: props => {
        const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
        return request && request.status !== 'STARTED';
    },
    load: props => props.DocActions.getDoc(props.match.params),
    LoadingComponent: LoadingPackageComponent
})(PackageComponent);