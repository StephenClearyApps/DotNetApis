import { Dispatch } from 'redux';

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

const TICK_UPDATE = 'TICK_UPDATE';
interface TickUpdateAction extends PayloadAction<{ timestamp: number }> { type: typeof TICK_UPDATE; };
function updateTick(timestamp: number): TickUpdateAction { return { type: TICK_UPDATE, payload: { timestamp }}; }

// Action functions.

const millisecondsPerMinute = 60 * 1000;
function currentMinute() {
    return Math.round(new Date().getTime() / millisecondsPerMinute) * millisecondsPerMinute;
}
const synchronize = (dispatch: Dispatch<TickUpdateAction>) => dispatch(updateTick(currentMinute()));
export const actions = {
    synchronize,
    startTicks: (dispatch: Dispatch<TickUpdateAction>) => {
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

function tickUpdate(state: State, action: TickUpdateAction): State {
    return {
        ...state,
        timestamp: action.payload.timestamp
    };
}

export function reducer(state: State = defaultState, action: Action): State {
    switch (action.type) {
        case TICK_UPDATE: return tickUpdate(state, action as TickUpdateAction); // TODO: as-cast is temporary until we have a union Action type.
        default: return state;
    }
}
