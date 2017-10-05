import * as React from 'react';

import { ReactComponent } from './util';

/** Executes an action when the component is mounted; the action is also fired whenever props change */
export const withExecuteOnMount =
    <TProps extends {}>(action: (state: TProps) => void) =>
    (Component: ReactComponent<TProps>) =>
    class ExecuteOnMount extends React.Component<TProps> {
        componentDidMount() { action(this.props); }
        componentWillReceiveProps(nextProps: TProps) { action(nextProps); }

        render() {
            return <Component {...this.props}/>;
        }
    };
