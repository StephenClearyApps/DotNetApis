import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router';

import { ReactComponent } from './util';
import { State } from '../../reducers';
import { packageKey, PackageDocumentationRequest } from '../../util';

export interface PackageRequestInjectedProps {
    pkgRequestKey: PackageKey;
    pkgRequestStatus: PackageDocumentationRequest;
}

const packageRequest =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageRequestInjectedProps>) =>
    (props: TComponentProps & State & RouteComponentProps<PackageKey>) => {
        const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
        return <Component {...props} pkgRequestStatus={request} pkgRequestKey={props.match.params}/>;
    };

/** Takes the route parameters `packageId`, `packageVersion`, and `targetFramework`; and injects `packageDocumentationRequest` */
export const withPackageRequest =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageRequestInjectedProps>) =>
    withRouter<TComponentProps & State>(packageRequest(Component));
