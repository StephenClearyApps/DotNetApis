import { Dispatch } from 'redux';

import * as actions from './actionTypes';
import * as api from './api';
import { listen } from './logic/log-listener';

export const DocActions = {
    getDoc: (key: PackageKey) => async (dispatch: Dispatch<any>) => {
        dispatch(actions.getDocBegin(key));
        try {
            const result = await api.getDoc(key);
            if (api.isInProgressResponse(result)) {
                const name = "log:" + result.normalizedPackageId + "/" + result.normalizedPackageVersion + "/" + result.normalizedFrameworkTarget;
                listen(name, (err, message, meta) => {
                    if (err) {
                        console.log("Ably error: " + err.message);
                    } else if (meta) {
                        console.log("Ably meta: " + meta);
                    } else {
                        console.log("Ably message: ", message);
                    }
                });
            } else {
                dispatch(actions.getDocEnd(key, result));
            }
        } catch (e) {
            dispatch(actions.getDocError(key, e));
        }
    }
}
