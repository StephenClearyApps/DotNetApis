import * as actionDispatchers from './ducks/_actions';
/** All action dispatchers */
export const actions = actionDispatchers;

// https://github.com/Microsoft/TypeScript/issues/14757
type ActionsAlias = typeof actions;
/** The type of all action dispatchers */
export interface Actions extends ActionsAlias {}