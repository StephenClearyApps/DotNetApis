import * as React from "react";

import { XmldocNode } from "./XmldocNode";

import { PackageDoc } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocExampesProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocExamples: React.StatelessComponent<XmldocExampesProps> = ({ data, pkg }) => {
    if (!data || !data.e)
        return null;
    return (
        <div>
            <h2>Examples</h2>
            {data.e.map((x, i) => <XmldocNode data={x} pkg={pkg} key={i}/>)}
        </div>
    );
}