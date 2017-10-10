import * as React from "react";

import { State } from "../reducers";
import { Actions } from "../actions";
import { withPackageRequestLoadOnDemand, PackageRequestInjectedProps } from "./hoc";

export interface PackageProps extends State, Actions {
}

function PackageComponent({ request, packageDoc }: PackageProps & PackageRequestInjectedProps) {
    const doc = packageDoc.packageDocumentation[request.normalizedPackageKey];
    return (
    <div>
        {JSON.stringify(doc)}
    </div>);
}

export const Package = withPackageRequestLoadOnDemand(PackageComponent);
