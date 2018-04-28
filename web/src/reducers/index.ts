import { combineReducers } from 'redux';

export * from './packageDocReducer';
export { PackageLogState } from '../ducks/packageLog';
import { packageDoc, PackageDocsState } from './packageDocReducer';
import { State as TimeState, reducer as time } from '../ducks/time';
import { State as PackageLogsState, reducer as packageLog } from '../ducks/packageLog';

export interface State {
    packageDoc: PackageDocsState;
    packageLog: PackageLogsState;
    time: TimeState;
}

export const reducers = combineReducers({
    packageDoc,
    packageLog,
    time
});
