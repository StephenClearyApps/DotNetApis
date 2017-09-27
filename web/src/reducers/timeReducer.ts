import { handleActions } from 'redux-actions';

import * as A from '../actionTypes';

export interface TimeState {
    /** The current timestamp, or 0 if the current timestamp isn't known. */
    timestamp: number;
}

function tick(state: TimeState, action: A.TickAction): TimeState {
    return {
        ...state,
        timestamp: action.payload.timestamp
    };
}

const defaultState: TimeState = {
    timestamp: 0,
};
export const time = handleActions({
    [A.ActionTypes.TICK]: tick
}, defaultState);
