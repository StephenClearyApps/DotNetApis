import { combineReducers } from 'redux';

import { State as PackageDocsState, reducer as packageDoc } from './ducks/packageDoc';
import { State as TimeState, reducer as time } from './ducks/time';
import { State as PackageLogsState, reducer as packageLog } from './ducks/packageLog';

export interface State {
    packageDoc: PackageDocsState;
    packageLog: PackageLogsState;
    time: TimeState;
}

export const reducers = combineReducers({
    packageDoc,
    packageLog,
    time
} as any); // Temporary any cast; once redux-thunk updates to support redux v4 typings, this should no longer be necessary.
