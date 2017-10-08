import { handleActions } from 'redux-actions';

import * as A from '../actionTypes';
import { packageKey, PackageDoc } from '../util';
import { LogMessage, Status as PackageStatus } from '../api';

export type Status = 'STARTED' | 'DONE' | 'ERROR';

interface PackageDocumentationRequest {
    /** The status of the request */
    status: Status;
    
    /** The request log (not backend log), if known */
    log?: LogMessage[];

    /** The streaming log (partial backend log); this is cleared out when the request completes */
    streamingLog?: LogMessage[];

    /** The normalized package key, if known; this is used as the key for `packageDocumentation` */
    normalizedPackageKey?: string;

    /** The error, if any */
    error?: Error;
}

interface PackageDocumentationRequests {
    [requestPackageKey: string]: PackageDocumentationRequest;
}

interface PackageDocumentationStatus {
    /** The status of the package documentation generation. */
    status: PackageStatus;

    /** The URI of the actual documentation. */
    jsonUri?: string;

    /** The URI of the backend processing log. */
    logUri?: string;

    /** The backend processing log, if loaded. */
    log?: LogMessage[];

    /** The documentation, if loaded. */
    json?: PackageDoc;
}

export interface PackageDocsState {
    /** The documentation for all known packages, indexed by normalized package key */
    packageDocumentation: {
        [normalizedPackageKey: string]: PackageDocumentationStatus;
    };

    /** The status of all package documentation requests, indexed by the request's package key */
    packageDocumentationRequests: PackageDocumentationRequests;
}

/** The "get doc" command has started */
function getDocBegin(state: PackageDocsState, action: A.GetDocBeginAction): PackageDocsState {
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [packageKey(action.meta.requestPackageKey)]: { status: 'STARTED' }
        }
    };
}

/** The "get doc" command has gotten to a point where it knows its normalized key */
function mapPackageKey(state: PackageDocsState, action: A.MapPackageKeyAction): PackageDocsState {
    const requestKey = packageKey(action.payload.requestPackageKey);
    const normalizedKey = packageKey(action.payload.normalizedPackageKey);
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                normalizedPackageKey: normalizedKey
            }
        },
        packageDocumentation: {
            ...state.packageDocumentation,
            [normalizedKey]: {
                status: 'Requested'
            }
        }
    };
}

/** The "get doc" command has completed with a redirection */
function getDocRedirecting(state: PackageDocsState, action: A.GetDocRedirectingAction): PackageDocsState {
    const requestKey = packageKey(action.meta.requestPackageKey);
    const normalizedKey = state.packageDocumentationRequests[requestKey].normalizedPackageKey;
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                log: action.payload.log
            }
        },
        packageDocumentation: {
            ...state.packageDocumentation,
            [normalizedKey]: {
                status: 'Succeeded',
                jsonUri: action.payload.jsonUri,
                logUri: action.payload.logUri
            }
        }
    };
}

/** The "get doc" command has been queued to the backend */
function getDocProcessing(state: PackageDocsState, action: A.GetDocProcessingAction): PackageDocsState {
    const requestKey = packageKey(action.meta.requestPackageKey);
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                log: action.payload.log,
                streamingLog: []
            }
        }
    };
}

// A progress report from the "get doc" command.
function getDocProgress(state: PackageDocsState, action: A.GetDocProgressAction): PackageDocsState {
    const requestKey = packageKey(action.meta.requestPackageKey);
    if (state.packageDocumentationRequests[requestKey].streamingLog === undefined)
        return;
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                streamingLog: [
                    ...state.packageDocumentationRequests[requestKey].streamingLog,
                    action.payload.logMessage
                ]
            }
        }
    }
}

/** We have received the documentation for the package */
function getDocEnd(state: PackageDocsState, action: A.GetDocEndAction): PackageDocsState {
    const requestKey = packageKey(action.meta.requestPackageKey);
    const normalizedKey = state.packageDocumentationRequests[requestKey].normalizedPackageKey;
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                status: 'DONE',
                streamingLog: undefined
            }
        },
        packageDocumentation: {
            ...state.packageDocumentation,
            [normalizedKey]: {
                ...state.packageDocumentation[normalizedKey],
                status: 'Succeeded',
                json: action.payload.data
            }
        }
    }
}

/** The "get doc" command failed on the backend */
function getDocBackendError(state: PackageDocsState, action: A.GetDocBackendErrorAction): PackageDocsState {
    const requestKey = packageKey(action.meta.requestPackageKey);
    const normalizedKey = state.packageDocumentationRequests[requestKey].normalizedPackageKey;
    return {
        ...state,
        packageDocumentation: {
            ...state.packageDocumentation,
            [normalizedKey]: {
                ...state.packageDocumentation[normalizedKey],
                status: 'Failed',
                logUri: action.payload.logUri
            }
        }
    };
}

/** Some part of the "get doc" command has failed */
function getDocError(state: PackageDocsState, action: A.GetDocErrorAction): PackageDocsState {
    const requestKey = packageKey(action.meta.requestPackageKey);
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                status: 'ERROR',
                error: action.payload
            }
        }
    };
}

const defaultState: PackageDocsState = {
    packageDocumentation: { },
    packageDocumentationRequests: { }
};
export const packageDoc = handleActions({
    [A.ActionTypes.GET_DOC_BEGIN]: getDocBegin,
    [A.ActionTypes.MAP_PACKAGE_KEY]: mapPackageKey,
    [A.ActionTypes.GET_DOC_REDIRECTING]: getDocRedirecting,
    [A.ActionTypes.GET_DOC_PROCESSING]: getDocProcessing,
    [A.ActionTypes.GET_DOC_PROGRESS]: getDocProgress,
    [A.ActionTypes.GET_DOC_END]: getDocEnd,
    [A.ActionTypes.GET_DOC_BACKEND_ERROR]: getDocBackendError,
    [A.ActionTypes.GET_DOC_ERROR]: getDocError
}, defaultState);
