import * as React from 'react';
import { CircularProgress } from 'material-ui';

export interface LoadOnDemandWrapperProps {
    /** Gets whether the desired item is loaded; this should return true if the item is loaded, loading, or errored. */
    isLoaded: () => boolean;

    /** Starts loading the desired item; this is only invoked if isLoaded returns false */
    load: () => void;

    /** The element to display while loading; if not specified, this component uses a CircularProgress */
    loadingElement?: React.ReactNode;
}

export class LoadOnDemandWrapper extends React.Component<LoadOnDemandWrapperProps> {
    componentDidMount() { this.requestDataIfNecessary(this.props); }
    componentWillReceiveProps(nextProps: LoadOnDemandWrapperProps) { this.requestDataIfNecessary(nextProps); }

    requestDataIfNecessary(props: LoadOnDemandWrapperProps) {
        // Only send request if the current store state is not loaded.
        const loaded = props.isLoaded();
        if (!loaded)
            props.load();
    }

    render() {
        const loaded = this.props.isLoaded();
        if (!loaded)
            return <div>{this.props.loadingElement || <CircularProgress/>}</div>;
        return <div>{this.props.children}</div>;
    }
}
