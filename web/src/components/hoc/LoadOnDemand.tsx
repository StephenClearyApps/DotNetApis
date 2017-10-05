import * as React from 'react';
import { compose } from 'recompose';

import { ReactComponent } from './util';
import { withExecuteOnMount } from './ExecuteOnMount';
import { withEither } from './Either';

interface LoadOnDemandOptions<T> {
    /** Gets whether the desired item is loaded; this should return true if the item is loaded, loading, or errored. */
    isLoaded: (state: T) => boolean;
    
    /** Starts loading the desired item; this is only invoked if isLoaded returns false */
    load: (state: T) => void;

    /** The component to display when the desired item is loading */
    LoadingComponent: ReactComponent<T>;
}

export const withLoadOnDemand =
    <TComponentProps extends {}>({ isLoaded, load, LoadingComponent } : LoadOnDemandOptions<TComponentProps>) =>
    (Component: ReactComponent<TComponentProps>) =>
    compose(
        withExecuteOnMount((state: TComponentProps) => { if (!isLoaded(state)) load(state); }),
        withEither(isLoaded, LoadingComponent)
    )(Component);
