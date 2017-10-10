import * as React from "react";

export { PackageInjectedProps } from './Package';
import { PackageInjectedProps } from './Package';
import { withPackageRequestLoadOnDemand, withPackage } from '.';
import { ReactComponent } from './util';

export const withAutoPackage =
    <TComponentProps extends {}>(Component: ReactComponent<TComponentProps & PackageInjectedProps>) =>
    withPackageRequestLoadOnDemand(withPackage(Component));
