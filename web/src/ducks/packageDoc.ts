import { Dispatch } from 'redux';
import * as api from '../api';
import { LogListener } from '../logic';
import { packageKey, PackageDoc, createAction, createMetaAction, createErrorAction } from '../util';
import { AllActions } from '.';

type LogMessage = api.LogMessage;
type PackageStatus = api.Status;

// Action strings, types, and factories.

const DOC_GET_BEGIN = 'doc/get/begin';
const docGetBeginAction = (requestPackageKey: PackageKey) => createMetaAction(DOC_GET_BEGIN, { requestPackageKey });
type DocGetBeginAction = ReturnType<typeof docGetBeginAction>;

const DOC_GET_MAPPACKAGEKEY = 'doc/get/mapPackageKey'
const docGetMapPackageKeyAction = (requestPackageKey: PackageKey, normalizedPackageKey: PackageKey) => createAction(DOC_GET_MAPPACKAGEKEY, { requestPackageKey, normalizedPackageKey });
type DocGetMapPackageKeyAction = ReturnType<typeof docGetMapPackageKeyAction>;

const DOC_GET_REDIRECT = 'doc/get/redirect';
const docGetRedirectAction = (requestPackageKey: PackageKey, log: LogMessage[], jsonUri: string, logUri: string) => createAction(DOC_GET_REDIRECT, { log, jsonUri, logUri }, { requestPackageKey });
type DocGetRedirectAction = ReturnType<typeof docGetRedirectAction>;

const DOC_GET_PROCESSING = 'doc/get/processing';
const docGetProcessAction = (requestPackageKey: PackageKey, log: LogMessage[]) => createAction(DOC_GET_PROCESSING, { log }, { requestPackageKey });
type DocGetProcessAction = ReturnType<typeof docGetProcessAction>;

const DOC_GET_PROGRESS = 'doc/get/progress';
const docGetReportProgressAction = (requestPackageKey: PackageKey, logMessage: LogMessage) => createAction(DOC_GET_PROGRESS, { logMessage }, { requestPackageKey });
type DocGetReportProgressAction = ReturnType<typeof docGetReportProgressAction>;

const DOC_GET_END = 'doc/get/end';
const docGetEndAction = (requestPackageKey: PackageKey, data: PackageDoc) => createAction(DOC_GET_END, { data }, { requestPackageKey });
type DocGetEndAction = ReturnType<typeof docGetEndAction>;

const DOC_GET_BACKENDERROR = 'doc/get/backendError';
const docGetBackendErrorAction = (requestPackageKey: PackageKey, logUri: string) => createAction(DOC_GET_BACKENDERROR, { logUri }, { requestPackageKey });
type DocGetBackendErrorAction = ReturnType<typeof docGetBackendErrorAction>;

const DOC_GET_ERROR = 'doc/get/error';
const docGetErrorAction = (requestPackageKey: PackageKey, error: Error) => createErrorAction(DOC_GET_ERROR, error, { requestPackageKey });
type DocGetErrorAction = ReturnType<typeof docGetErrorAction>;

export type Actions = DocGetBeginAction | DocGetMapPackageKeyAction | DocGetRedirectAction | DocGetProcessAction | DocGetReportProgressAction | DocGetEndAction | DocGetBackendErrorAction | DocGetErrorAction;

// Action dispatchers

export const actions = {
    getDoc: (requestKey: PackageKey) => async (dispatch: Dispatch<any>) => {
        dispatch(docGetBeginAction(requestKey));
        try {
            const docGetResponse = await api.getDoc(requestKey);

            // A non-error response always includes normalization information
            const normalizedKey = { packageId: docGetResponse.normalizedPackageId, packageVersion: docGetResponse.normalizedPackageVersion, targetFramework: docGetResponse.normalizedFrameworkTarget };
            dispatch(docGetMapPackageKeyAction(requestKey, normalizedKey));

            if (!api.isInProgressResponse(docGetResponse)) {
                // Response is a redirect response
                const redirectResponse = docGetResponse as api.RedirectResponse;
                dispatch(docGetRedirectAction(requestKey, redirectResponse.log, redirectResponse.jsonUri, redirectResponse.logUri));
                const doc = await api.getPackageDocumentation(redirectResponse.jsonUri);
                dispatch(docGetEndAction(requestKey, PackageDoc.create(doc)));
                return;
            }

            // Response is a processing response.
            dispatch(docGetProcessAction(requestKey, docGetResponse.log));
            const channelName = "log:" + packageKey(normalizedKey);
            const listener = new LogListener(channelName, (err, message, meta) => {
                if (err) {
                    dispatch(docGetReportProgressAction(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: "Streaming log error: " + err.message }));
                } else if (meta) {
                    dispatch(docGetReportProgressAction(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: meta }));
                } else {
                    dispatch(docGetReportProgressAction(requestKey, { type: message!.name, timestamp: message!.timestamp, message: message!.data.message }));
                }
            });
            try {
                listener.listen();
                while (true) {
                    await Promise.delay(2000);
                    const pollResult = await api.getStatus(normalizedKey);
                    if (pollResult.status === "Succeeded") {
                        dispatch(docGetRedirectAction(requestKey, docGetResponse.log, pollResult.jsonUri, pollResult.logUri));
                        const doc = await api.getPackageDocumentation(pollResult.jsonUri);
                        dispatch(docGetEndAction(requestKey, PackageDoc.create(doc)));
                        return;
                    } else if (pollResult.status === "Failed") {
                        dispatch(docGetBackendErrorAction(requestKey, pollResult.logUri));
                        return;
                    } else {
                        dispatch(docGetReportProgressAction(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: "Documentation processing status: " + pollResult.status}));
                    }
                }
            } finally {
                listener.dispose();
            }
        } catch (e) {
            dispatch(docGetErrorAction(requestKey, e));
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
function docGetBegin(state: State, action: DocGetBeginAction): State {
    return {
        ...state,
        packageDocumentationRequests: {
            ...state.packageDocumentationRequests,
            [packageKey(action.meta.requestPackageKey)]: { status: 'STARTED' }
        }
    };
}

/** The "get doc" command has gotten to a point where it knows its normalized key */
function docGetMapPackageKey(state: State, action: DocGetMapPackageKeyAction): State {
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
function docGetRedirect(state: State, action: DocGetRedirectAction): State {
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
function docGetProcess(state: State, action: DocGetProcessAction): State {
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
function docGetReportProgress(state: State, action: DocGetReportProgressAction): State {
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
function docGetEnd(state: State, action: DocGetEndAction): State {
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
function docGetBackendError(state: State, action: DocGetBackendErrorAction): State {
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
function docGetError(state: State, action: DocGetErrorAction): State {
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

export function reducer(state: State = defaultState, action: AllActions): State {
    switch (action.type) {
        case DOC_GET_BEGIN: return docGetBegin(state, action);
        case DOC_GET_MAPPACKAGEKEY: return docGetMapPackageKey(state, action);
        case DOC_GET_REDIRECT: return docGetRedirect(state, action);
        case DOC_GET_PROCESSING: return docGetProcess(state, action);
        case DOC_GET_PROGRESS: return docGetReportProgress(state, action);
        case DOC_GET_END: return docGetEnd(state, action);
        case DOC_GET_BACKENDERROR: return docGetBackendError(state, action);
        case DOC_GET_ERROR: return docGetError(state, action);
        default: return state;
    }
}
