import { Dispatch } from 'redux';
import * as api from '../api';

type LogMessage = api.LogMessage;

// Action strings, types, and creators.

const GETLOG_BEGIN = 'packageLog/getLog/BEGIN';
interface GetLogBeginAction extends MetaAction<{ normalizedPackageKey: string }> { type: typeof GETLOG_BEGIN; };
function getLogBeginAction(normalizedPackageKey: string): GetLogBeginAction { return { type: GETLOG_BEGIN, meta: { normalizedPackageKey }}; }

const GETLOG_END = 'packageLog/getLog/END';
interface GetLogEndAction extends MetaPayloadAction<{ normalizedPackageKey: string }, { log: LogMessage[] }> { type: typeof GETLOG_END; };
function getLogEndAction(normalizedPackageKey: string, log: LogMessage[]): GetLogEndAction { return { type: GETLOG_END, meta: { normalizedPackageKey }, payload: { log }}; }

const GETLOG_ERROR = 'packageLog/getLog/ERROR';
interface GetLogErrorAction extends MetaErrorAction<{ normalizedPackageKey: string }> { type: typeof GETLOG_ERROR; };
function getLogErrorAction(normalizedPackageKey: string, error: Error): GetLogErrorAction { return { type: GETLOG_ERROR, meta: { normalizedPackageKey }, payload: error, error: true }; }

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

export function reducer(state: State = defaultState, action: Action): State {
    switch (action.type) {
        case GETLOG_BEGIN: return getLogBegin(state, action as GetLogBeginAction); // TODO: as-cast is temporary until we have a union Action type.
        case GETLOG_END: return getLogEnd(state, action as GetLogEndAction);
        case GETLOG_ERROR: return getLogError(state, action as GetLogErrorAction);
        default: return state;
    }
}
