import * as React from 'react';

/** Displays a component only while a given predicate returns true */
export const createMaybe =
    <TProps extends {}>(predicate: (props: TProps) => boolean) =>
    (Component: React.ComponentType<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : null;
