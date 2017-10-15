import { handleActions } from 'redux-actions';

import * as A from '../actionTypes';
import { packageKey } from '../util';
import { LogMessage, Status as PackageStatus } from '../api';

export type Status = 'STARTED' | 'DONE' | 'ERROR';

export interface PackageLogState {
    /** The status of the request */
    status: Status;
    
    /** The error, if any */
    error?: Error;

    /** The logs, if any */
    log?: LogMessage[];
}

export interface PackageLogsState {
    /** The log requests for all known packages, indexed by normalized package key */
    packageLogs: {
        [normalizedPackageKey: string]: PackageLogState;
    };
}

/** The "get log" command has started */
function getLogBegin(state: PackageLogsState, action: A.GetLogBeginAction): PackageLogsState {
    return {
        ...state,
        packageLogs: {
            ...state.packageLogs,
            [packageKey(action.meta.normalizedPackageKey)]: { status: 'STARTED' }
        }
    };
}

/** We have received the log for the package */
function getLogEnd(state: PackageLogsState, action: A.GetLogEndAction): PackageLogsState {
    const key = packageKey(action.meta.normalizedPackageKey);
    return {
        ...state,
        packageLogs: {
            ...state.packageLogs,
            [key]: {
                ...state.packageLogs[key],
                status: 'DONE',
                log: action.payload.log
            }
        }
    }
}

/** Some part of the "get log" command has failed */
function getLogError(state: PackageLogsState, action: A.GetLogErrorAction): PackageLogsState {
    const key = packageKey(action.meta.normalizedPackageKey);
    return {
        ...state,
        packageLogs: {
            ...state.packageLogs,
            [key]: {
                ...state.packageLogs[key],
                status: 'ERROR',
                error: action.payload
            }
        }
    };
}

const defaultState: PackageLogsState = {
    packageLogs: { }
};
export const packageLog = handleActions({
    [A.ActionTypes.GET_LOG_BEGIN]: getLogBegin,
    [A.ActionTypes.GET_LOG_END]: getLogEnd,
    [A.ActionTypes.GET_LOG_ERROR]: getLogError
}, defaultState);
