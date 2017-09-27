import { handleActions } from 'redux-actions';

import * as A from '../actionTypes';
import { packageKey } from '../util/packageKey';
import { PackageDoc } from '../util/packageDoc';

export type Status = 'STARTED' | 'DONE' | 'ERROR';

export interface LogMessage {
    type?: string;
    timestamp?: number;
    message: string;
}

export interface PackageDocsState {
    packages: {
        [key: string]: PackageDoc;
    };
    mapping: {
        [key: string]: {
            status: Status;
            key?: string;
        }
    };
    logs: {
        [key: string]: LogMessage[];
    };
}

// The "get doc" command has started
function getDocBegin(state: PackageDocsState, action: A.GetDocBeginAction): PackageDocsState {
    return {
        ...state,
        mapping: {
            ...state.mapping,
            [packageKey(action.meta.key)]: { status: 'STARTED' }
        }
    };
}

// The "get doc" command has been queued to the backend; we know the normalized key at this point.
function getDocProcessing(state: PackageDocsState, action: A.GetDocProcessingAction): PackageDocsState {
    return {
        ...state,
        mapping: {
            ...state.mapping,
            [packageKey(action.meta.key)]: { status: 'STARTED', key: packageKey(action.meta.normalized)}
        },
        logs: {
            ...state.logs,
            [packageKey(action.meta.normalized)]: action.payload.log.map(x => ({ message: x }))
        }
    };
}

// A progress report from the "get doc" command.
function getDocProgress(state: PackageDocsState, action: A.GetDocProgressAction): PackageDocsState {
    const key = packageKey(action.meta.normalized);
    return {
        ...state,
        logs: {
            ...state.logs,
            [key]: [
                ...state.logs[key],
                action.payload
            ]
        }
    }
}

const defaultState: PackageDocsState = {
    packages: {},
    mapping: {},
    logs: {}
};
export const packageDoc = handleActions({
    [A.ActionTypes.GET_DOC_BEGIN]: getDocBegin,
    [A.ActionTypes.GET_DOC_PROCESSING]: getDocProcessing,
    [A.ActionTypes.GET_DOC_PROGRESS]: getDocProgress
}, defaultState);
