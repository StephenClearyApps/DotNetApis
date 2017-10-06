import * as React from "react";
import { match, RouteComponentProps } from "react-router";

import { LoadingMessage } from "./LoadingMessage";
import { State } from "../reducers";
import { Actions } from "../actions";
import { withLoadOnDemand } from "./hoc";
import { packageKey } from "../util/packageKey";

interface RouteParams {
    packageId: string;
    packageVersion: string;
    packageTarget: string;
}

function PackageComponent(props: State & Actions & RouteComponentProps<RouteParams>)
{
    console.log(props);
    const mapping = props.packageDoc.mapping[packageKey(props.match.params)];
    if (mapping.status === 'ERROR')
        return <div>TODO: error display</div>;
    const doc = props.packageDoc.packages[mapping.key];
    return (
    <div>
        {JSON.stringify(doc)}
    </div>);
}

export const Package = withLoadOnDemand<State & Actions & RouteComponentProps<RouteParams>>({
    hasStarted: props => {
        const mapping = props.packageDoc.mapping[packageKey(props.match.params)];
        return !!mapping;
    },
    isLoaded: props => {
        const mapping = props.packageDoc.mapping[packageKey(props.match.params)];
        return mapping && mapping.status !== 'STARTED';
    },
    load: props => props.DocActions.getDoc(props.match.params),
    LoadingComponent: props => <LoadingMessage message={"Loading documentation for " + props.match.params.packageId} />
})(PackageComponent);