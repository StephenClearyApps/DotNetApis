import * as React from 'react';
import { CircularProgress } from 'material-ui';
import { State } from '../reducers';

type ReactComponent<T = {}> = React.ComponentClass<T> | React.StatelessComponent<T>;

function compose(...funcs: Array<(component: ReactComponent) => ReactComponent>) {
    if (funcs.length === 0) {
        return (component: ReactComponent) => component;
    }

    if (funcs.length === 1) {
        return funcs[0];
    }

    return funcs.reduce((a, b) => (component: ReactComponent) => a(b(component)));
}

/** Displays a component only while a given predicate returns true */
export const withMaybe =
    <TProps extends {}>(predicate: (state: TProps) => boolean) =>
    (Component: ReactComponent<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : null;

/** Displays either one component or another, depending on a given predicate */
export const withEither =
    <TProps extends {}>(predicate: (state: TProps) => boolean, FalseComponent: ReactComponent<TProps>) =>
    (Component: ReactComponent<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : <FalseComponent {...props}/>;

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

interface LoadOnDemandOptions<T> {
    /** Gets whether the desired item is loaded; this should return true if the item is loaded, loading, or errored. */
    isLoaded: (state: T) => boolean;
    
    /** Starts loading the desired item; this is only invoked if isLoaded returns false */
    load: (state: T) => void;

    /** The component to display when the desired item is loading */
    LoadingComponent: ReactComponent<T>;
}

/** Determines if a desired item is loaded, and if not, loads it while showing a LoadingComponent. */
export const withLoadOnDemand =
    <TComponentProps extends {}>({ isLoaded, load, LoadingComponent } : LoadOnDemandOptions<TComponentProps>) =>
    (Component: ReactComponent<TComponentProps>) =>
    withExecuteOnMount((state: TComponentProps) => { if (!isLoaded(state)) load(state); })(
        withEither(isLoaded, LoadingComponent)(Component)
    );

export const withLoadOnDemand2 =
    <TComponentProps extends {}>({ isLoaded, load, LoadingComponent } : LoadOnDemandOptions<TComponentProps>) =>
    (Component: ReactComponent<TComponentProps>) =>
    compose(
        withExecuteOnMount((state: TComponentProps) => { if (!isLoaded(state)) load(state); }),
        withEither(isLoaded, LoadingComponent)
    )(Component);
