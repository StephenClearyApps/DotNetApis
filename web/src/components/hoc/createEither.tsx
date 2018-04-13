import * as React from 'react';

import { Hoc } from '.';

/** Displays either one component or another, depending on a given predicate */
export function createEither<TProps>(predicate: (props: TProps) => boolean, FalseComponent: React.ComponentType<TProps>): Hoc<TProps> {
    return Component => props =>
    predicate(props) ? <Component {...props}/> : <FalseComponent {...props}/>;
}
