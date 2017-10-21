import * as React from 'react';

import { PackageLoading } from '../PackageLoading';
import { PackageRequestError } from '../PackageRequestError';

import { State } from '../../reducers';
import { Actions } from '../../actions';
import { ReactComponent } from './util';
import { PackageRequestInjectedProps, withPackageRequest } from './PackageRequest';
import { PackageLogRequestInjectedProps, withPackageLogRequest } from './PackageLogRequest';
import { withLoadOnDemand } from './LoadOnDemand';
import { withEither } from './Either';
import { PackageInjectedProps, withPackage } from './Package';
import { PackageLogLoading } from '../PackageLogLoading';
import { PackageLogRequestError } from '../PackageLogRequestError';

const loadPackageOnDemand = withLoadOnDemand<State & Actions & PackageRequestInjectedProps>({
    hasStarted: props => !!props.pkgRequestStatus,
    isLoaded: props => props.pkgRequestStatus && props.pkgRequestStatus.status !== 'STARTED',
    load: props => props.DocActions.getDoc(props.pkgRequestKey),
    LoadingComponent: PackageLoading
});

const loadLogOnDemand = withLoadOnDemand<State & Actions & PackageLogRequestInjectedProps>({
    hasStarted: props => !!props.pkgLogRequestStatus,
    isLoaded: props => props.pkgLogRequestStatus && props.pkgLogRequestStatus.status !== 'STARTED',
    load: props => { props.DocActions.getDocLog(props.pkgRequestStatus.normalizedPackageKey, props.pkgStatus.logUri); },
    LoadingComponent: PackageLogLoading
})

const packageError = withEither<State & Actions & PackageRequestInjectedProps & PackageLogRequestInjectedProps>(props => props.pkgRequestStatus.status !== 'ERROR', PackageRequestError);
const logError = withEither<State & Actions & PackageLogRequestInjectedProps>(props => props.pkgLogRequestStatus.status !== 'ERROR', PackageLogRequestError);

/** Loads a package and its log on demand, and handles displaying package loading and package request error components */
export const withPackageLogLoadOnDemand =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & State & Actions & PackageLogRequestInjectedProps>) =>
    withPackageRequest(loadPackageOnDemand(packageError(withPackage(withPackageLogRequest(loadLogOnDemand(logError(Component)))))));
