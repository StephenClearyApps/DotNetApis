import * as React from 'react';

import { Hoc } from '.';

/** Displays a component only while a given predicate returns true */
export function createMaybe<TProps>(predicate: (props: TProps) => boolean): Hoc<TProps> {
    return Component => props =>
    predicate(props) ? <Component {...props}/> : null;
}
