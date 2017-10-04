import * as React from 'react';

import { Status } from '../reducers/packageDocReducer';

export interface LoadOnDemandWrapperProps {
    /** Gets the current status of the desired item */
    getStatus: () => Status | undefined;

    /** Starts loading the desired item; this is only invoked if getStatus returns undefined */
    load: () => void;

    loadingElement: React.ReactNode;
    errorElement: React.ReactNode;
    valueElement: React.ReactNode;
}

export class LoadOnDemandWrapper extends React.Component<LoadOnDemandWrapperProps> {
    componentDidMount() { this.requestDataIfNecessary(this.props); }
    componentWillReceiveProps(nextProps: LoadOnDemandWrapperProps) { this.requestDataIfNecessary(nextProps); }

    requestDataIfNecessary(props: LoadOnDemandWrapperProps) {
        // Only send request if the current store state is not loaded.
        const status = props.getStatus();
        if (!status)
            props.load();
    }

    render() {
        const status = this.props.getStatus();
        if (!status || status === 'STARTED')
            return <div>{this.props.loadingElement}</div>;
        if (status === 'ERROR')
            return <div>{this.props.errorElement}</div>;
        return <div>{this.props.valueElement}</div>;
    }
}
