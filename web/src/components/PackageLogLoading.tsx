import * as React from 'react';
import CircularProgress from '@material-ui/core/CircularProgress';

import { State } from "../reducers";
import { PackageLogRequestInjectedProps } from "./hoc";

export type PackageLogLoadingProps = State & PackageLogRequestInjectedProps;

export const PackageLogLoading: React.StatelessComponent<PackageLogLoadingProps> = props => {
    const { pkgRequestStatus, pkgStatus } = props;
    return (
        <div style={{textAlign: "center"}}>
            <h1>{"Loading processing log for " + pkgRequestStatus.normalizedPackageKey + " from " + pkgStatus.logUri}</h1>
            <div style={{marginTop: "1em", marginBottom: "1em"}}><CircularProgress/></div>
        </div>
    );
}