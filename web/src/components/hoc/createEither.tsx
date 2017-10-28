import * as React from 'react';

import { ReactComponent } from '.';

/** Displays either one component or another, depending on a given predicate */
export const createEither =
    <TProps extends {}>(predicate: (props: TProps) => boolean, FalseComponent: ReactComponent<TProps>) =>
    (Component: ReactComponent<TProps>) =>
    (props: TProps) => predicate(props) ? <Component {...props}/> : <FalseComponent {...props}/>;