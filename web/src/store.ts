import { applyMiddleware, createStore } from 'redux';
import immutableState from 'redux-immutable-state-invariant';
import { composeWithDevTools } from 'redux-devtools-extension';
import thunk from 'redux-thunk';

import { reducers, State } from './reducers';

declare var process : {
    env: {
        NODE_ENV: string
    }
}

const middlewareEnhancer = process.env.NODE_ENV !== 'production' ? applyMiddleware(immutableState(), thunk) : applyMiddleware(thunk);
const storeEnhancer = composeWithDevTools<State, {}>(middlewareEnhancer);

const factory = storeEnhancer(createStore);
export const store = factory(reducers);