import { actions as TimeActions } from './ducks/time';
import { actions as PackageLogActions } from './ducks/packageLog';
import { actions as PackageDocActions } from './ducks/packageDoc';

export const actions = {
    TimeActions,
    PackageLogActions,
    PackageDocActions
};

export type Actions = typeof actions;
