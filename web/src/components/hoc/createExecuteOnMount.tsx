import * as React from 'react';

import { lifecycle } from '.';

/** Executes an action when the component is mounted; the action is also fired whenever props change */
export const createExecuteOnMount =
    <TProps extends {}>(action: (props: TProps) => void) =>
    (Component: React.ComponentType<TProps>) =>
    lifecycle<TProps, void>({
        componentDidMount: function() { action(this.props); },
        componentWillReceiveProps: props => action(props)
    })(Component) as any as React.ComponentType<TProps>;
