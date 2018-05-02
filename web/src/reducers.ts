import { State as PackageDocsState, reducer as packageDoc } from './ducks/packageDoc';
import { State as TimeState, reducer as time } from './ducks/time';
import { State as PackageLogsState, reducer as packageLog } from './ducks/packageLog';
import { AllActions } from './ducks';
import { AnyAction } from 'redux';

export interface State {
    packageDoc: PackageDocsState;
    packageLog: PackageLogsState;
    time: TimeState;
}

function rootReducer(state: State, action: AllActions): State {
    return {
        packageDoc: packageDoc(state ? state.packageDoc : undefined, action),
        packageLog: packageLog(state ? state.packageLog : undefined, action),
        time: time(state ? state.time : undefined, action)
    };
}

export function reducers(state: State, action: AnyAction): State {
    return rootReducer(state, action as AllActions);
}
