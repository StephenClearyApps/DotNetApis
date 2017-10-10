import * as React from "react";

import { ReactComponent } from './util';
import { State, PackageDocumentationStatus } from "../../reducers";
import { PackageDoc } from "../../util/packageDoc";
import { PackageRequestInjectedProps } from "./PackageRequest";

export interface PackageInjectedProps {
    packageStatus: PackageDocumentationStatus;
    pkg: PackageDoc;
}

export const withPackage =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageInjectedProps>) =>
    (props: TComponentProps & State & PackageRequestInjectedProps) => {
        const packageStatus = props.packageDoc.packageDocumentation[props.request.normalizedPackageKey];
        return <Component {...props} packageStatus={packageStatus} pkg={packageStatus.json}/>;
    };
