import * as React from 'react';
import { Hoc } from '.';

/** Displays either one component or another, depending on a given predicate */
export function createEither<TProps>(predicate: (props: TProps) => boolean, FalseComponent: React.ComponentType<TProps>): Hoc {
    let TFalseComponent: any = FalseComponent; // we pass extra props
    return TrueComponent => props => predicate(props as any) ? <TrueComponent {...props}/> : <TFalseComponent {...props}/>;
}