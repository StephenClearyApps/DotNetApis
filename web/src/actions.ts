import { Dispatch } from 'redux';

import * as actions from './actionTypes';
import * as api from './api';

export const DocActions = {
    getDoc: (key: PackageKey) => async (dispatch: Dispatch<any>) => {
        dispatch(actions.getDocBegin(key));
        try {
            const result = await api.getDoc();
            dispatch(actions.getDocEnd(key, result));
        } catch (e) {
            dispatch(actions.getDocError(key, e));
        }
    }
}
