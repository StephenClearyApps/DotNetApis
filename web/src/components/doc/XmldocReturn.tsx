import * as React from "react";

import { XmldocNode } from "./XmldocNode";

import { PackageDoc } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocReturnProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocReturn: React.StatelessComponent<XmldocReturnProps> = ({ data, pkg }) => {
    if (!data || !data.r)
        return null;
    return (
        <div>
            <h2>Return Value</h2>
            <XmldocNode data={data.r} pkg={pkg}/>)
        </div>
    );
}