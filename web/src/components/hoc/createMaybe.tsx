import * as React from 'react';

import { ReactComponent } from '.';

/** Displays a component only while a given predicate returns true */
export const createMaybe =
    <TProps extends {}>(predicate: (props: TProps) => boolean) =>
    (Component: ReactComponent<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : null;
