import * as React from 'react';

import { lifecycle, Hoc } from '.';

/** Executes an action when the component is mounted; the action is also fired whenever props change */
export function createExecuteOnMount<TProps>(action: (props: TProps) => void): Hoc<TProps> {
    return lifecycle<TProps, void>({
        componentDidMount: function() { action(this.props); },
        componentWillReceiveProps: props => action(props)
    });
}