import * as React from 'react';
import { ReactFragment } from '../fragments/partial';
import { ResponseError, isResponseError, ErrorDetails as ErrorDetailsType, ExceptionDetails } from '../api';
import { LogMessages } from './LogMessages';

function exceptionDetails(error: ExceptionDetails): ReactFragment {
    return [
        [<h3>Backend exception details</h3>],
        [<pre>[{error.exceptionType}]: {error.exceptionMessage}</pre>],
        [<pre>{error.stackTrace}</pre>],
        error.innerException ? exceptionDetails(error.innerException) : null
    ];
}

function backendErrorDetails(error: ErrorDetailsType, currentTimestamp: number): ReactFragment {
    return [
        [<h3>Backend error details</h3>],
        [<p>Operation ID: <code>{error.operationId}</code></p>],
        [<p>Request ID: <code>{error.requestId}</code></p>],
        exceptionDetails(error),
        [<h3>Backend trace log</h3>],
        [<LogMessages messages={error.log} currentTimestamp={currentTimestamp}/>]
    ];
}

function javascriptErrorDetails(error: Error, currentTimestamp: number): ReactFragment {
    if (!error)
        return null;
    if (!isResponseError(error)) {
        return [
            [<pre>{error.message}</pre>],
            [<h3>Client-side stack</h3>],
            [<pre>{error.stack}</pre>]
        ];
    }
    const result: ReactFragment[] = [[<pre>{error.status} {error.message}</pre>]];
    if (error.details) {
        result.push(backendErrorDetails(error.details, currentTimestamp));
    }
    result.push([
        [<h3>Client-side stack</h3>],
        [<pre>{error.stack}</pre>]
    ]);
    return result;
}

export interface ErrorDetailsProps {
    error: Error,
    currentTimestamp: number
}

export const ErrorDetails: React.StatelessComponent<ErrorDetailsProps> = props => {
    return <div className="errorDetails">{javascriptErrorDetails(props.error, props.currentTimestamp)}</div>;
}
