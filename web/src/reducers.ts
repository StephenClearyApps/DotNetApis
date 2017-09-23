import { combineReducers } from 'redux';
import { handleActions } from 'redux-actions';

import * as A from './actionTypes';
import { packageKey } from './util/packageKey';

export interface PackageDocState {
    packages: {
        [key: string]: any; // TODO: any
    };
    mapping: {
        [key: string]: {
            status: string;
            key?: string;
        }
    };
}

export interface State {
    packageDoc: PackageDocState;
}

function getDocBegin(state: PackageDocState, action: A.GetDocBeginAction): PackageDocState {
    return {...state, mapping: {...state.mapping, [packageKey(action.meta.key)]: { status: 'Started' }}};
}

const packageDoc = handleActions({
    [A.ActionTypes.GET_DOC_BEGIN]: getDocBegin
}, { packages: {}, mapping: {} });

export const reducers = combineReducers({
    packageDoc
});
