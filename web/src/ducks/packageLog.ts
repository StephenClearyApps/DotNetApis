import { Dispatch } from 'redux';
import * as api from '../api';
import { createAction, createMetaAction, createErrorAction } from '../util';
import { AllActions } from '.';

type LogMessage = api.LogMessage;

// Action strings, types, and factories.

const LOG_GET_BEGIN = 'log/get/begin';
const logGetBeginAction = (normalizedPackageKey: string) => createMetaAction(LOG_GET_BEGIN, { normalizedPackageKey });
type LogGetBeginAction = ReturnType<typeof logGetBeginAction>;

const LOG_GET_END = 'log/get/end';
const logGetEndAction = (normalizedPackageKey: string, log: LogMessage[]) => createAction(LOG_GET_END, { log }, { normalizedPackageKey });
type LogGetEndAction = ReturnType<typeof logGetEndAction>;

const LOG_GET_ERROR = 'log/get/error';
const logGetErrorAction = (normalizedPackageKey: string, error: Error) => createErrorAction(LOG_GET_ERROR, error, { normalizedPackageKey });
type LogGetErrorAction = ReturnType<typeof logGetErrorAction>;

export type Actions = LogGetBeginAction | LogGetEndAction | LogGetErrorAction;

// Action dispatchers

export const actions = {
    getLog: (normalizedKey: string, logUri: string) => async (dispatch: Dispatch<Actions>) => {
        dispatch(logGetBeginAction(normalizedKey));
        try {
            const result = await api.getPackageLog(logUri);
            dispatch(logGetEndAction(normalizedKey, result));
        } catch (e) {
            dispatch(logGetErrorAction(normalizedKey, e));
        }
    }    
};

// State

type PackageLogRequestStatus = 'STARTED' | 'DONE' | 'ERROR';
export interface PackageLogState {
    /** The status of the request */
    readonly status: PackageLogRequestStatus;
    
    /** The error, if any */
    readonly error?: Error;

    /** The logs, if any */
    readonly log?: LogMessage[];
}
export interface State {
    /** The log requests for all known packages, indexed by normalized package key */
    readonly packageLogs: {
        [normalizedPackageKey: string]: PackageLogState;
    };
}
const defaultState: State = {
    packageLogs: { }
};

// Reducers

/** The "get log" command has started */
function logGetBegin(state: State, action: LogGetBeginAction): State {
    return {
        ...state,
        packageLogs: {
            ...state.packageLogs,
            [action.meta.normalizedPackageKey]: { status: 'STARTED' }
        }
    };
}

/** We have received the log for the package */
function logGetEnd(state: State, action: LogGetEndAction): State {
    return {
        ...state,
        packageLogs: {
            ...state.packageLogs,
            [action.meta.normalizedPackageKey]: {
                ...state.packageLogs[action.meta.normalizedPackageKey],
                status: 'DONE',
                log: action.payload.log
            }
        }
    }
}

/** Some part of the "get log" command has failed */
function logGetError(state: State, action: LogGetErrorAction): State {
    return {
        ...state,
        packageLogs: {
            ...state.packageLogs,
            [action.meta.normalizedPackageKey]: {
                ...state.packageLogs[action.meta.normalizedPackageKey],
                status: 'ERROR',
                error: action.payload
            }
        }
    };
}

export function reducer(state: State = defaultState, action: AllActions): State {
    switch (action.type) {
        case LOG_GET_BEGIN: return logGetBegin(state, action);
        case LOG_GET_END: return logGetEnd(state, action);
        case LOG_GET_ERROR: return logGetError(state, action);
        default: return state;
    }
}
