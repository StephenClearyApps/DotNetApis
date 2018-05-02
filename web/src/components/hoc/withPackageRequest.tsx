import * as React from 'react';

import { RouteComponentProps, Hoc } from '.';
import { State } from '../../reducers';
import { packageKey, PackageDocumentationRequest } from '../../util';

export interface PackageRequestInjectedProps {
    pkgRequestKey: PackageKey;
    pkgRequestStatus: PackageDocumentationRequest;
}
export type PackageRequestRequiredProps = State & RouteComponentProps<PackageKey>;

function createWithPackageRequest<TProps>(): Hoc<PackageRequestInjectedProps, PackageRequestRequiredProps> {
    return Component => props => {
        const request = props.packageDoc.packageDocumentationRequests[packageKey(props.match.params)];
        return <Component {...props} pkgRequestStatus={request} pkgRequestKey={props.match.params}/>;
    };
}

/** Takes the route parameters `PackageKey`, and injects `PackageRequestInjectedProps` */
export const withPackageRequest = createWithPackageRequest();
