import * as React from 'react';
import { lifecycle } from 'recompose';

import { ReactComponent } from './util';

/** Executes an action when the component is mounted; the action is also fired whenever props change */
export const withExecuteOnMount =
    <TProps extends {}>(action: (state: TProps) => void) =>
    (Component: ReactComponent<TProps>) =>
    lifecycle<TProps, {}>({
        componentDidMount: () => action(this.props),
        componentWillReceiveProps: (props) => action(props)
    })(Component);
