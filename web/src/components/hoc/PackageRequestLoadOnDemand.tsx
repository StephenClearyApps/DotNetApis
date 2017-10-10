import * as React from 'react';
import { CircularProgress } from 'material-ui';

import { PackageLoading } from '../PackageLoading';
import { PackageError } from '../PackageError';

import { State } from '../../reducers';
import { Actions } from '../../actions';
import { ReactComponent } from './util';
import { PackageRequestInjectedProps, withPackageRequest } from './PackageRequest';
import { withLoadOnDemand } from './LoadOnDemand';
import { withEither } from './Either';

const loadOnDemand = withLoadOnDemand<State & Actions & PackageRequestInjectedProps>({
    hasStarted: props => !!props.request,
    isLoaded: props => props.request && props.request.status !== 'STARTED',
    load: props => props.DocActions.getDoc(props.requestParams),
    LoadingComponent: PackageLoading
});

const eitherError = withEither<State & Actions & PackageRequestInjectedProps>(props => props.request.status !== 'ERROR', PackageError);

/** Loads a package on demand, and handles displaying package loading and package error components */
export const withPackageRequestLoadOnDemand =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & State & Actions & PackageRequestInjectedProps>) =>
    withPackageRequest(loadOnDemand(eitherError(Component)));