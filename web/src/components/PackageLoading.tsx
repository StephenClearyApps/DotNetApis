import * as React from 'react';
import CircularProgress from 'material-ui/CircularProgress';

import { PackageRequestLog } from "./PackageRequestLog";

import { State } from "../reducers";
import { PackageRequestInjectedProps } from "./hoc";
import { packageFriendlyName } from '../util';

export type PackageLoadingProps = State & PackageRequestInjectedProps;

export const PackageLoading: React.StatelessComponent<PackageLoadingProps> = props => {
    const { pkgRequestKey } = props;
    return (
        <div style={{textAlign: "center"}}>
            <h1>{"Loading documentation for " + packageFriendlyName(pkgRequestKey)}</h1>
            <PackageRequestLog {...props}/>
            <div style={{marginTop: "1em", marginBottom: "1em"}}><CircularProgress/></div>
        </div>
    );
}