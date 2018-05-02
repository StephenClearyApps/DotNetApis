import { lifecycle } from 'recompose';

import { Hoc } from '.';

/** Executes an action when the component is mounted; the action is also fired whenever props change */
export function createExecuteOnMount<TProps>(action: (props: TProps) => void): Hoc {
    return lifecycle<TProps, {}>({
        componentDidMount: function() { action(this.props); },
        componentWillReceiveProps: props => action(props)
    }) as any;
}
