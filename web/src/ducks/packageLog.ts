import { Dispatch } from 'redux';
import * as api from '../api';
import { createAction, createMetaAction, createErrorAction } from '../util';

type LogMessage = api.LogMessage;

// Action strings, types, and creators.

const GETLOG_BEGIN = 'packageLog/getLog/BEGIN';
const getLogBeginAction = (normalizedPackageKey: string) => createMetaAction(GETLOG_BEGIN, { normalizedPackageKey });
type GetLogBeginAction = ReturnType<typeof getLogBeginAction>;

const GETLOG_END = 'packageLog/getLog/END';
const getLogEndAction = (normalizedPackageKey: string, log: LogMessage[]) => createAction(GETLOG_END, { log }, { normalizedPackageKey });
type GetLogEndAction = ReturnType<typeof getLogEndAction>;

const GETLOG_ERROR = 'packageLog/getLog/ERROR';
const getLogErrorAction = (normalizedPackageKey: string, error: Error) => createErrorAction(GETLOG_ERROR, error, { normalizedPackageKey });
type GetLogErrorAction = ReturnType<typeof getLogErrorAction>;

type Actions = GetLogBeginAction | GetLogEndAction | GetLogErrorAction;

// Actions

export const actions = {
    getLog: (normalizedKey: string, logUri: string) => async (dispatch: Dispatch<any>) => {
        dispatch(getLogBeginAction(normalizedKey));
        try {
            const result = await api.getPackageLog(logUri);
            dispatch(getLogEndAction(normalizedKey, result));
        } catch (e) {
            dispatch(getLogErrorAction(normalizedKey, e));
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
function getLogBegin(state: State, action: GetLogBeginAction): State {
    return {
        ...state,
        packageLogs: {
            ...state.packageLogs,
            [action.meta.normalizedPackageKey]: { status: 'STARTED' }
        }
    };
}

/** We have received the log for the package */
function getLogEnd(state: State, action: GetLogEndAction): State {
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
function getLogError(state: State, action: GetLogErrorAction): State {
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

export function reducer(state: State = defaultState, action: Actions): State {
    switch (action.type) {
        case GETLOG_BEGIN: return getLogBegin(state, action);
        case GETLOG_END: return getLogEnd(state, action);
        case GETLOG_ERROR: return getLogError(state, action);
        default: return state;
    }
}
