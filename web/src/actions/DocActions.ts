import { Dispatch } from 'redux';

import * as actions from '../actionTypes';
import * as api from '../api';
import { LogListener } from '../logic/log-listener';
import { packageKey } from '../util/packageKey';
import { PackageDoc } from '../util/packageDoc';
import { IPackage } from '../util/structure/packages';

export const DocActions = {
    getDoc: (key: PackageKey) => async (dispatch: Dispatch<any>) => {
        dispatch(actions.getDocBegin(key));
        try {
            const result = await api.getDoc(key);
            if (!api.isInProgressResponse(result)) {
                dispatch(actions.getDocEnd(key, PackageDoc.create(result)));
                return;
            }
            const normalizedKey = { packageId: result.normalizedPackageId, packageVersion: result.normalizedPackageVersion, targetFramework: result.normalizedFrameworkTarget };
            dispatch(actions.getDocProcessing(key, normalizedKey, result.log));
            const channelName = "log:" + packageKey(normalizedKey);
            const listener = new LogListener(channelName, (err, message, meta) => {
                if (err) {
                    dispatch(actions.getDocProgress(normalizedKey, "meta", (new Date).getTime(), "Streaming log error: " + err.message));
                } else if (meta) {
                    dispatch(actions.getDocProgress(normalizedKey, "meta", (new Date).getTime(), meta));
                } else {
                    dispatch(actions.getDocProgress(normalizedKey, message.name, message.timestamp, message.data.message));
                }
            });
            try {
                listener.listen();
                while (true) {
                    await Promise.delay(2000);
                    const pollResult = await api.getStatus(normalizedKey.packageId, normalizedKey.packageVersion,
                        normalizedKey.targetFramework, result.timestamp);
                    if (pollResult.status === "Succeeded") {
                        dispatch(actions.getDocEnd(key, PackageDoc.create(await api.getJson<IPackage>(pollResult.jsonUri))));
                        return;
                    } else if (pollResult.status === "Failed") {
                        dispatch(actions.getDocError(key, new Error("Log failed; see " + pollResult.logUri)));
                        return;
                    }
                }
            } finally {
                listener.dispose();
            }
        } catch (e) {
            dispatch(actions.getDocError(key, e));
        }
    }
}
