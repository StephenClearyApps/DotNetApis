import * as React from 'react';

/** Displays either one component or another, depending on a given predicate */
export const createEither =
    <TProps extends {}>(predicate: (props: TProps) => boolean, FalseComponent: React.ComponentType<TProps>) =>
    (Component: React.ComponentType<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : <FalseComponent {...props}/>;