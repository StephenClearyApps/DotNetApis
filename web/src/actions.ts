import { Dispatch } from 'redux';

import * as actions from './actionTypes';
import * as api from './api';
import { listen } from './logic/log-listener';
import { packageKey } from './util/packageKey';
import { PackageDoc } from './util/packageDoc';

export const DocActions = {
    getDoc: (key: PackageKey) => async (dispatch: Dispatch<any>) => {
        dispatch(actions.getDocBegin(key));
        try {
            const result = await api.getDoc(key);
            if (api.isInProgressResponse(result)) {
                const normalizedKey = { packageId: result.normalizedPackageId, packageVersion: result.normalizedPackageVersion, targetFramework: result.normalizedFrameworkTarget };
                dispatch(actions.getDocProcessing(key, normalizedKey, result.log));
                // TODO: start polling status API
                const channelName = "log:" + packageKey(normalizedKey);
                listen(channelName, (err, message, meta) => {
                    if (err) {
                        dispatch(actions.getDocProgress(normalizedKey, "meta", (new Date).getTime(), "Streaming log error: " + err.message));
                    } else if (meta) {
                        dispatch(actions.getDocProgress(normalizedKey, "meta", (new Date).getTime(), meta));
                    } else {
                        dispatch(actions.getDocProgress(normalizedKey, message.name, message.timestamp, message.data.message));
                    }
                });
            } else {
                dispatch(actions.getDocEnd(key, PackageDoc.create(result)));
            }
        } catch (e) {
            dispatch(actions.getDocError(key, e));
        }
    }
}
