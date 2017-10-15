import { combineReducers } from 'redux';

export * from './packageDocReducer';
import { packageDoc, PackageDocsState } from './packageDocReducer';
import { packageLog, PackageLogsState } from './packageLogReducer';
import { time, TimeState } from './timeReducer';

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
