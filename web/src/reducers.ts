import { AllActions, State, rootReducer } from './ducks';
import { AnyAction } from 'redux';

export { State };

export function reducers(state: State, action: AnyAction): State {
    return rootReducer(state, action as AllActions);
}
