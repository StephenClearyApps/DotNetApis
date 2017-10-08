import * as React from "react";

import { Xmldoc } from "./Xmldoc";

import { PackageDoc } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocExampesProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocExampes: React.StatelessComponent<XmldocExampesProps> = ({ data, pkg }) => {
    if (!data || !data.e)
        return null;
    return (
        <div>
            <h2>Examples</h2>
            {data.e.map((x, i) => <Xmldoc data={x} pkg={pkg} key={i}/>)}
        </div>
    );
}