import * as React from "react";

import { State } from "../reducers";
import { Actions } from "../actions";
import { withPackageRequestLoadOnDemand, withPackage, PackageInjectedProps } from "./hoc";

export interface PackageProps extends State, Actions {
}

function PackageComponent({ pkg, packageDoc }: PackageProps & PackageInjectedProps) {
    return (
    <div>
        {JSON.stringify(pkg)}
    </div>);
}

export const Package = withPackageRequestLoadOnDemand(withPackage(PackageComponent));
