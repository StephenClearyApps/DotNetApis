import { Dispatch } from 'redux';

import * as actions from '../actionTypes';
import * as api from '../api';
import { LogListener } from '../logic/log-listener';
import { packageKey } from '../util/packageKey';
import { PackageDoc } from '../util/packageDoc';
import { IPackage } from '../util/structure/packages';

export const DocActions = {
    getDoc: (requestKey: PackageKey) => async (dispatch: Dispatch<any>) => {
        dispatch(actions.getDocBegin(requestKey));
        try {
            const getDocResponse = await api.getDoc(requestKey);

            // A non-error response always includes normalization information
            const normalizedKey = { packageId: getDocResponse.normalizedPackageId, packageVersion: getDocResponse.normalizedPackageVersion, targetFramework: getDocResponse.normalizedFrameworkTarget };
            dispatch(actions.mapPackageKey(requestKey, normalizedKey));

            if (!api.isInProgressResponse(getDocResponse)) {
                // Response is a redirect response
                const redirectResponse = getDocResponse as api.RedirectResponse;
                dispatch(actions.getDocRedirecting(requestKey, redirectResponse.log));
                // TODO: save backend log uri in state
                const doc = await api.getPackageDocumentation(redirectResponse.jsonUri);
                dispatch(actions.getDocEnd(requestKey, PackageDoc.create(doc)));
                return;
            }

            // Response is a processing response.
            dispatch(actions.getDocProcessing(requestKey, getDocResponse.log));
            const channelName = "log:" + packageKey(normalizedKey);
            const listener = new LogListener(channelName, (err, message, meta) => {
                if (err) {
                    dispatch(actions.getDocProgress(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: "Streaming log error: " + err.message }));
                } else if (meta) {
                    dispatch(actions.getDocProgress(requestKey, { type: "meta", timestamp: (new Date).getTime(), message: meta }));
                } else {
                    dispatch(actions.getDocProgress(requestKey, { type: message.name, timestamp: message.timestamp, message: message.data.message }));
                }
            });
            try {
                listener.listen();
                while (true) {
                    await Promise.delay(2000);
                    const pollResult = await api.getStatus(normalizedKey.packageId, normalizedKey.packageVersion,
                        normalizedKey.targetFramework, getDocResponse.timestamp);
                    if (pollResult.status === "Succeeded") {
                        // TODO: save backend log uri in state
                        const doc = await api.getPackageDocumentation(pollResult.jsonUri);
                        dispatch(actions.getDocEnd(requestKey, PackageDoc.create(doc)));
                        return;
                    } else if (pollResult.status === "Failed") {
                        // TODO: save backend log uri in state
                        dispatch(actions.getDocError(requestKey, new Error("Log failed; see " + pollResult.logUri)));
                        return;
                    }
                }
            } finally {
                listener.dispose();
            }
        } catch (e) {
            dispatch(actions.getDocError(requestKey, e));
        }
    }
}