import { Dispatch } from 'redux';
import { createAction } from '../util';

// Naming conventions from https://medium.com/@kylpo/redux-best-practices-eef55a20cc72
// Ducks from https://github.com/erikras/ducks-modular-redux
// Typing from https://github.com/piotrwitek/react-redux-typescript-guide and https://github.com/piotrwitek/typesafe-actions#tutorial

// Action strings are named `{NOUN}_{VERB}`
// Action types are named `{Noun}{Verb}Action`, should be an interface type extending one of the generic action types, and specify a `type` of `typeof {NOUN_VERB}`.
// Action creators are named `{verb}{Noun}` and return `{Noun}{Verb}Action`.
// Duck module exports an `actions` that contains higher-level actions.
// Duck module exports a `State` type.
// State properties are `readonly`.
// Duck module exports a `reducer` function.

// Action strings, types, and creators.

const TICK = 'time/TICK';
const tickAction = (timestamp: number) => createAction(TICK, { timestamp });
type TickAction = ReturnType<typeof tickAction>;

type Actions = TickAction;

// Action functions.

const millisecondsPerMinute = 60 * 1000;
function currentMinute() {
    return Math.round(new Date().getTime() / millisecondsPerMinute) * millisecondsPerMinute;
}
const synchronize = (dispatch: Dispatch<TickAction>) => dispatch(tickAction(currentMinute()));
export const actions = {
    synchronize,
    startTicks: (dispatch: Dispatch<TickAction>) => {
        synchronize(dispatch);
        setInterval(() => synchronize(dispatch), millisecondsPerMinute);
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

function tick(state: State, action: TickAction): State {
    return {
        ...state,
        timestamp: action.payload.timestamp
    };
}

export function reducer(state: State = defaultState, action: Actions): State {
    switch (action.type) {
        case TICK: return tick(state, action);
        default: return state;
    }
}
