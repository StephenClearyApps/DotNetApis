import { actions as TimeActions } from './ducks/time';
import { actions as PackageLogActions } from './ducks/packageLog';
import { actions as PackageDocActions } from './ducks/packageDoc';

export const actions = {
    TimeActions,
    PackageLogActions,
    PackageDocActions
};

// https://github.com/Microsoft/TypeScript/issues/14757
type ActionsAlias = typeof actions;
export interface Actions extends ActionsAlias {}
