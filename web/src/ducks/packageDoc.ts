import { Dispatch } from 'redux';
import * as api from '../api';
import { LogListener } from '../logic';
import { packageKey, PackageDoc, createAction, createMetaAction } from '../util';

type LogMessage = api.LogMessage;
type PackageStatus = api.Status;

// Action strings, types, and creators.

const GETDOC_BEGIN = 'packageDoc/getDoc/BEGIN';
const getDocBeginAction = (requestPackageKey: PackageKey) => createMetaAction(GETDOC_BEGIN, { requestPackageKey });
type GetDocBeginAction = ReturnType<typeof getDocBeginAction>;

const GETDOC_MAPPACKAGEKEY = 'packageDoc/getDoc/MAPPACKAGEKEY'
const getDocMapPackageKeyAction = (requestPackageKey: PackageKey, normalizedPackageKey: PackageKey) => createAction(GETDOC_MAPPACKAGEKEY, { requestPackageKey, normalizedPackageKey });
type GetDocMapPackageKeyAction = ReturnType<typeof getDocMapPackageKeyAction>;

const GETDOC_REDIRECT = 'packageDoc/getDoc/REDIRECT';
const getDocRedirectAction = (requestPackageKey: PackageKey, log: LogMessage[], jsonUri: string, logUri: string) => createAction(GETDOC_REDIRECT, { log, jsonUri, logUri }, { requestPackageKey });
type GetDocRedirectAction = ReturnType<typeof getDocRedirectAction>;

const GETDOC_PROCESS = 'packageDoc/getDoc/PROCESS';
const getDocProcessAction = (requestPackageKey: PackageKey, log: LogMessage[]) => createAction(GETDOC_PROCESS, { log }, { requestPackageKey });
type GetDocProcessAction = ReturnType<typeof getDocProcessAction>;

const GETDOC_REPORTPROGRESS = 'packageDoc/getDoc/REPORTPROGRESS';
const getDocReportProgressAction = (requestPackageKey: PackageKey, logMessage: LogMessage) => createAction(GETDOC_REPORTPROGRESS, { logMessage }, { requestPackageKey });
type GetDocReportProgressAction = ReturnType<typeof getDocReportProgressAction>;

const GETDOC_END = 'packageDoc/getDoc/END';
const getDocEndAction = (requestPackageKey: PackageKey, data: PackageDoc) => createAction(GETDOC_END, { data }, { requestPackageKey });
type GetDocEndAction = ReturnType<typeof getDocEndAction>;

const GETDOC_BACKENDERROR = 'packageDoc/getDoc/BACKENDERROR';
const getDocBackendErrorAction = (requestPackageKey: PackageKey, logUri: string) => createAction(GETDOC_BACKENDERROR, { logUri }, { requestPackageKey });
type GetDocBackendErrorAction = ReturnType<typeof getDocBackendErrorAction>;

const GETDOC_ERROR = 'packageDoc/getDoc/ERROR';
const getDocErrorAction = (requestPackageKey: PackageKey, error: Error) => createAction(GETDOC_ERROR, error, { requestPackageKey });
type GetDocErrorAction = ReturnType<typeof getDocErrorAction>;

type Actions = GetDocBeginAction | GetDocMapPackageKeyAction | GetDocRedirectAction | GetDocProcessAction | GetDocReportProgressAction | GetDocEndAction | GetDocBackendErrorAction | GetDocErrorAction;

// Actions

export const actions = {
    getDoc: (requestKey: PackageKey) => async (dispatch: Dispatch<any>) => {
        dispatch(getDocBeginAction(requestKey));
        try {
            const getDocResponse = await api.getDoc(requestKey);

            // A non-error response always includes normalization information
            const normalizedKey = { packageId: getDocResponse.normalizedPackageId, packageVersion: getDocResponse.normalizedPackageVersion, targetFramework: getDocResponse.normalizedFrameworkTarget };
            dispatch(getDocMapPackageKeyAction(requestKey, normalizedKey));

            if (!api.isInProgressResponse(getDocResponse)) {
                // Response is a redirect response
                const redirectResponse = getDocResponse as api.RedirectResponse;
                dispatch(getDocRedirectAction(requestKey, redirectResponse.log, redirectResponse.jsonUri, redirectResponse.logUri));
                const doc = await api.getPackageDocumentation(redirectResponse.jsonUri);
                dispatch(getDocEndAction(requestKey, PackageDoc.create(doc)));
                return;
            }

            // Response is a processing response.
            dispatch(getDocProcessAction(requestKey, getDocResponse.log));
            const channelName = "log:" + packageKey(normalizedKey);
            const listener = new LogListener(channelName, (err, message, meta) => {
                if (err) {
                    dispatch(getDocReportProgressAction(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: "Streaming log error: " + err.message }));
                } else if (meta) {
                    dispatch(getDocReportProgressAction(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: meta }));
                } else {
                    dispatch(getDocReportProgressAction(requestKey, { type: message!.name, timestamp: message!.timestamp, message: message!.data.message }));
                }
            });
            try {
                listener.listen();
                while (true) {
                    await Promise.delay(2000);
                    const pollResult = await api.getStatus(normalizedKey);
                    if (pollResult.status === "Succeeded") {
                        dispatch(getDocRedirectAction(requestKey, getDocResponse.log, pollResult.jsonUri, pollResult.logUri));
                        const doc = await api.getPackageDocumentation(pollResult.jsonUri);
                        dispatch(getDocEndAction(requestKey, PackageDoc.create(doc)));
                        return;
                    } else if (pollResult.status === "Failed") {
                        dispatch(getDocBackendErrorAction(requestKey, pollResult.logUri));
                        return;
                    } else {
                        dispatch(getDocReportProgressAction(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: "Documentation processing status: " + pollResult.status}));
                    }
                }
            } finally {
                listener.dispose();
            }
        } catch (e) {
            dispatch(getDocErrorAction(requestKey, e));
        }
    }
};

// State

type PackageDocRequestStatus = 'STARTED' | 'DONE' | 'ERROR' | 'BACKEND_ERROR';
export interface PackageDocumentationRequest {
    /** The status of the request */
    readonly status: PackageDocRequestStatus;
    
    /** The request log (not backend log), if known */
    readonly log?: LogMessage[];

    /** The streaming log (partial backend log); this is cleared out when the request completes */
    readonly streamingLog?: LogMessage[];

    /** The normalized package key, if known; this is used as the key for `packageDocumentation` */
    readonly normalizedPackageKey?: string;

    /** The error, if any */
    readonly error?: Error;
}
interface PackageDocumentationRequests {
    readonly [requestPackageKey: string]: PackageDocumentationRequest;
}
export interface PackageDocumentationStatus {
    /** The status of the package documentation generation. */
    readonly status: PackageStatus;

    /** The URI of the actual documentation. */
    readonly jsonUri?: string;

    /** The URI of the backend processing log. */
    readonly logUri?: string;

    /** The documentation, if loaded. */
    readonly json?: PackageDoc;
}
export interface State {
    /** The documentation for all known packages, indexed by normalized package key */
    readonly packageDocumentation: {
        readonly [normalizedPackageKey: string]: PackageDocumentationStatus;
    };

    /** The status of all package documentation requests, indexed by the request's package key */
    readonly packageDocumentationRequests: PackageDocumentationRequests;
}
const defaultState: State = {
    packageDocumentation: { },
    packageDocumentationRequests: { }
};

// Reducers

/** The "get doc" command has started */
function getDocBegin(state: State, action: GetDocBeginAction): State {
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [packageKey(action.meta.requestPackageKey)]: { status: 'STARTED' }
        }
    };
}

/** The "get doc" command has gotten to a point where it knows its normalized key */
function getDocMapPackageKey(state: State, action: GetDocMapPackageKeyAction): State {
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
function getDocRedirect(state: State, action: GetDocRedirectAction): State {
    const requestKey = packageKey(action.meta.requestPackageKey);
    const normalizedKey = state.packageDocumentationRequests[requestKey].normalizedPackageKey!;
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
function getDocProcess(state: State, action: GetDocProcessAction): State {
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
function getDocReportProgress(state: State, action: GetDocReportProgressAction): State {
    const requestKey = packageKey(action.meta.requestPackageKey);
    const streamingLog = state.packageDocumentationRequests[requestKey].streamingLog;
    if (streamingLog === undefined)
        return state;
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                streamingLog: [
                    ...streamingLog,
                    action.payload.logMessage
                ]
            }
        }
    }
}

/** We have received the documentation for the package */
function getDocEnd(state: State, action: GetDocEndAction): State {
    const requestKey = packageKey(action.meta.requestPackageKey);
    const normalizedKey = state.packageDocumentationRequests[requestKey].normalizedPackageKey!;
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
function getDocBackendError(state: State, action: GetDocBackendErrorAction): State {
    const requestKey = packageKey(action.meta.requestPackageKey);
    const normalizedKey = state.packageDocumentationRequests[requestKey].normalizedPackageKey!;
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [requestKey]: {
                ...state.packageDocumentationRequests[requestKey],
                status: 'BACKEND_ERROR',
                error: new Error("Documentation generation failed; see the processing log for details.")
            }
        },
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
function getDocError(state: State, action: GetDocErrorAction): State {
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

export function reducer(state: State = defaultState, action: Actions): State {
    switch (action.type) {
        case GETDOC_BEGIN: return getDocBegin(state, action);
        case GETDOC_MAPPACKAGEKEY: return getDocMapPackageKey(state, action);
        case GETDOC_REDIRECT: return getDocRedirect(state, action);
        case GETDOC_PROCESS: return getDocProcess(state, action);
        case GETDOC_REPORTPROGRESS: return getDocReportProgress(state, action);
        case GETDOC_END: return getDocEnd(state, action);
        case GETDOC_BACKENDERROR: return getDocBackendError(state, action);
        case GETDOC_ERROR: return getDocError(state, action);
        default: return state;
    }
}
