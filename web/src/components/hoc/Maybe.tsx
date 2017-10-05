import * as React from 'react';

import { ReactComponent } from './util';

/** Displays a component only while a given predicate returns true */
export const withMaybe =
    <TProps extends {}>(predicate: (state: TProps) => boolean) =>
    (Component: ReactComponent<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : null;
