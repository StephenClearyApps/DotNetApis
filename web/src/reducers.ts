import { combineReducers } from 'redux';

import { packageDoc, PackageDocsState } from './reducers/packageDocReducer';
import { time, TimeState } from './reducers/timeReducer';

export interface State {
    packageDoc: PackageDocsState;
    time: TimeState;
}

export const reducers = combineReducers({
    packageDoc,
    time
});
