import * as React from "react";

import { Xmldoc } from "./Xmldoc";

import { PackageDoc, IXmldoc } from "../../util";

interface XmldocRemarksProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocRemarks: React.StatelessComponent<XmldocRemarksProps> = ({ data, pkg }) => {
    if (!data || !data.m)
        return null;
    return (
        <div>
            <h2>Remarks</h2>
            <Xmldoc data={data.m} pkg={pkg}/>)
        </div>
    );
}