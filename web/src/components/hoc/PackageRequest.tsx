import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router';

import { ReactComponent } from './util';
import { State, PackageDocumentationRequest } from '../../reducers';
import { packageKey } from '../../util';

interface RouteParams {
    packageId: string;
    packageVersion?: string;
    targetFramework?: string;
}

export interface PackageRequestInjectedProps {
    request: PackageDocumentationRequest;
    requestParams: RouteParams;
}

const packageRequest =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageRequestInjectedProps>) =>
    (props: TComponentProps & State & RouteComponentProps<RouteParams>) => {
        const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
        return <Component {...props} request={request} requestParams={props.match.params}/>
    };

/** Takes the route parameters `packageId`, `packageVersion`, and `targetFramework`; and injects `packageDocumentationRequest` */
export const withPackageRequest =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageRequestInjectedProps>) =>
    withRouter<TComponentProps & State>(packageRequest(Component));
