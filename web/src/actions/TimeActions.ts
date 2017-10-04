import { Dispatch } from 'redux';

import * as actions from '../actionTypes';

const millisecondsPerMinute = 60 * 1000;
function currentMinute() {
    return Math.round(new Date().getTime() / millisecondsPerMinute) * millisecondsPerMinute;
}

export const TimeActions = {
    startTicks: (dispatch: Dispatch<actions.TickAction>) => {
        dispatch(actions.getTick(currentMinute()));
        setInterval(() => dispatch(actions.getTick(currentMinute())), millisecondsPerMinute);
    }
}
