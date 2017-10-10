import { combineReducers } from 'redux';

export * from './packageDocReducer';
import { packageDoc, PackageDocsState } from './packageDocReducer';
import { time, TimeState } from './timeReducer';

export interface State {
    packageDoc: PackageDocsState;
    time: TimeState;
}

export const reducers = combineReducers({
    packageDoc,
    time
});
