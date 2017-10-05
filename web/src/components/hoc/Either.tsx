import * as React from 'react';

import { ReactComponent } from './util';

/** Displays either one component or another, depending on a given predicate */
export const withEither =
    <TProps extends {}>(predicate: (state: TProps) => boolean, FalseComponent: ReactComponent<TProps>) =>
    (Component: ReactComponent<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : <FalseComponent {...props}/>;
