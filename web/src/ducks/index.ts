import { Actions as DocActions } from './packageDoc';
import { Actions as LogActions } from "./packageLog";
import { Actions as TimeActions } from "./time";

export type AllActions = DocActions | LogActions | TimeActions;