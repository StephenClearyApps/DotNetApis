import { combineReducers } from 'redux';

export { PackageLogState } from '../ducks/packageLog'; // TODO: move
export { PackageDocumentationStatus, PackageDocumentationRequest } from '../ducks/packageDoc'; // TODO: move
import { State as PackageDocsState, reducer as packageDoc } from '../ducks/packageDoc';
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
