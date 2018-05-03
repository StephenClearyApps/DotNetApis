import { Actions as PackageDocActions, State as PackageDocsState, reducer as packageDoc } from './packageDoc';
import { Actions as PackageLogActions, State as PackageLogsState, reducer as packageLog } from "./packageLog";
import { Actions as TimeActions, State as TimeState, reducer as time } from "./time";

/** Union type of all application actions */
export type AllActions = PackageDocActions | PackageLogActions | TimeActions;

/** Composite application state */
export interface State {
    packageDoc: PackageDocsState;
    packageLog: PackageLogsState;
    time: TimeState;
}

/** The root application reducer */
export function rootReducer(state: State, action: AllActions): State {
    return {
        packageDoc: packageDoc(state ? state.packageDoc : undefined, action),
        packageLog: packageLog(state ? state.packageLog : undefined, action),
        time: time(state ? state.time : undefined, action)
    };
}