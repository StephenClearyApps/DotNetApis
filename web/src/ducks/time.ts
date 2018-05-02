import { Dispatch } from 'redux';
import { createAction } from '../util';
import { AllActions } from '.';

// Naming conventions from https://medium.com/@kylpo/redux-best-practices-eef55a20cc72
// Ducks from https://github.com/erikras/ducks-modular-redux
// Typing from https://github.com/piotrwitek/react-redux-typescript-guide and https://github.com/piotrwitek/typesafe-actions#tutorial
//  and https://hackernoon.com/redux-flow-type-getting-the-maximum-benefit-from-the-fewest-key-strokes-5c006c54ec87

// Verbs are in present tense.
// Action types are a `type` alias, set equal to `ReturnType<typeof actionCreator>;`
// "Action methods" are higher-level actions that take a Dispatch<T> argument.
// Duck module exports an `actions` that contains action methods.
// Duck module exports a `State` type.
// State properties are `readonly`.
// Duck module exports a `reducer` function.
// Naming:
//  Action strings: "{noun}/{verb}[/{notification}]", e.g., "log/get/begin", "doc/get/redirect", "time/synchronize"
//  Action constants: {NOUN}_{VERB}[_{NOTIFICATION}], e.g., LOG_GET_BEGIN, DOC_GET_REDIRECT, TIME_SYNCHRONIZE
//  Action types: {Noun}{Verb}[{Notification}]Action, e.g., LogGetBeginAction, DocGetRedirectAction, TimeSynchronizeAction
//  Action factories: {noun}{Verb}[{Notification}]Action, e.g., logGetBeginAction, docGetRedirectAction, timeSynchronizeAction
//  Action dispatchers: {verb}{Noun}, e.g., getLog, getDoc, synchronizeTime, startTimeSynchronization
//  Reducer functions: {noun}{Verb}[{Notification}], e.g., logGetBegin, docGetRedirect, timeSynchronize

// Action strings, types, and factories.

const TIME_SYNCHRONIZE = 'time/synchronize';
const timeSynchronizeAction = (timestamp: number) => createAction(TIME_SYNCHRONIZE, { timestamp });
type TimeSynchronizeAction = ReturnType<typeof timeSynchronizeAction>;

export type Actions = TimeSynchronizeAction;

// Action dispatchers.

const millisecondsPerMinute = 60 * 1000;
function currentMinute() {
    return Math.round(new Date().getTime() / millisecondsPerMinute) * millisecondsPerMinute;
}
const synchronizeTime = (dispatch: Dispatch<TimeSynchronizeAction>) => dispatch(timeSynchronizeAction(currentMinute()));
export const actions = {
    synchronizeTime,
    startTimeSynchronization: (dispatch: Dispatch<TimeSynchronizeAction>) => {
        synchronizeTime(dispatch);
        setInterval(() => synchronizeTime(dispatch), millisecondsPerMinute);
    }
}

// State.

export interface State {
    /** The current timestamp, or 0 if the current timestamp isn't known. */
    readonly timestamp: number;
}
const defaultState: State = {
    timestamp: 0,
};

// Reducers.

function timeSynchronize(state: State, action: TimeSynchronizeAction): State {
    return {
        ...state,
        timestamp: action.payload.timestamp
    };
}

export function reducer(state: State = defaultState, action: AllActions): State {
    switch (action.type) {
        case TIME_SYNCHRONIZE: return timeSynchronize(state, action);
        default: return state;
    }
}
