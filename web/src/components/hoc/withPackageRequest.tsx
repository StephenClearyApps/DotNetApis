import * as React from 'react';

import { RouteComponentProps } from '.';
import { State } from '../../reducers';
import { packageKey, PackageDocumentationRequest } from '../../util';

export interface PackageRequestInjectedProps {
    pkgRequestKey: PackageKey;
    pkgRequestStatus: PackageDocumentationRequest;
}

/** Takes the route parameters `PackageKey`, and injects `PackageRequestInjectedProps` */
export const withPackageRequest =
    <TProps extends {}>(Component: React.ComponentType<TProps & PackageRequestInjectedProps>) =>
    (props: TProps & State & RouteComponentProps<PackageKey>) => {
        const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
        return <Component {...props} pkgRequestStatus={request} pkgRequestKey={props.match.params}/>;
    };
