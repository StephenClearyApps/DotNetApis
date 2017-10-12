import { Dispatch } from 'redux';

import * as actions from '../actionTypes';

const millisecondsPerMinute = 60 * 1000;
function currentMinute() {
    return Math.round(new Date().getTime() / millisecondsPerMinute) * millisecondsPerMinute;
}

const synchronize = (dispatch: Dispatch<actions.TickAction>) =>
dispatch(actions.getTick(currentMinute()));

export const TimeActions = {
    synchronize,

    startTicks: (dispatch: Dispatch<actions.TickAction>) => {
        synchronize(dispatch);
        setInterval(() => synchronize(dispatch), millisecondsPerMinute);
    }
}
