import * as React from "react";

import { State } from "../reducers";
import { Actions } from "../actions";
import { withPackageRequestLoadOnDemand, PackageRequestInjectedProps } from "./hoc";

function PackageComponent({ request, packageDoc }: State & Actions & PackageRequestInjectedProps) {
    const doc = packageDoc.packageDocumentation[request.normalizedPackageKey];
    return (
    <div>
        {JSON.stringify(doc)}
    </div>);
}

export const Package = withPackageRequestLoadOnDemand(PackageComponent);
