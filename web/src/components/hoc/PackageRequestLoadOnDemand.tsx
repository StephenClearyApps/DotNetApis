import * as React from 'react';
import { CircularProgress } from 'material-ui';

import { ReactComponent } from './util';
import { PackageRequestInjectedProps, withPackageRequest } from './PackageRequest';
import { State, PackageDocumentationRequest } from '../../reducers';
import { LogMessages } from '../LogMessages';
import { withLoadOnDemand } from './LoadOnDemand';
import { Actions } from '../../actions';
import { withEither } from './Either';

function LoadingPackageComponent({ time, request, requestParams }: State & PackageRequestInjectedProps) {
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

function ErrorPackageComponent({ request }: PackageRequestInjectedProps) {
    if (request.status === 'ERROR')
        return <div>TODO: error display</div>;
}

const loadOnDemand = withLoadOnDemand<State & Actions & PackageRequestInjectedProps>({
    hasStarted: props => !!props.request,
    isLoaded: props => props.request && props.request.status !== 'STARTED',
    load: props => props.DocActions.getDoc(props.requestParams),
    LoadingComponent: LoadingPackageComponent
});

const eitherError = withEither<State & Actions & PackageRequestInjectedProps>(props => props.request.status !== 'ERROR', ErrorPackageComponent);

/** Loads a package on demand, and handles displaying package loading and package error components */
export const withPackageRequestLoadOnDemand =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & State & Actions & PackageRequestInjectedProps>) =>
    withPackageRequest(loadOnDemand(eitherError(Component)));