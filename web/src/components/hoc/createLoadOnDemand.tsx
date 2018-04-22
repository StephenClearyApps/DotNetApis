import * as React from 'react';

import { createExecuteOnMount, Hoc } from '.';

interface LoadOnDemandOptions<TProps> {
    /** Gets whether the desired item has started loading; this should return true if the item is loading, loaded, or errored. */
    hasStarted: (props: TProps) => boolean;
    
    /** Starts loading the desired item; this is only invoked if hasStarted returns false */
    load: (props: TProps) => void;
}

/** Starts loading a resource automatically when the component is mounted. */
export function createLoadOnDemand<TProps>({ hasStarted, load }: LoadOnDemandOptions<TProps>): Hoc<TProps> {
    const withExecuteOnMount = createExecuteOnMount<TProps>(props => { if (!hasStarted(props)) load(props); });
    return (Component: React.ComponentType<TProps>) => withExecuteOnMount(Component);
}