import { applyMiddleware, createStore, GenericStoreEnhancer } from 'redux';
import immutableState from 'redux-immutable-state-invariant';
import { composeWithDevTools } from 'redux-devtools-extension';

import { reducers } from './reducers';

const middlewareEnhancer = process.env.NODE_ENV !== 'production' ? applyMiddleware(immutableState()) : undefined;
const storeEnhancer = composeWithDevTools(middlewareEnhancer);

const factory = storeEnhancer(createStore);
export const store = factory(reducers);