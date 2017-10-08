import * as React from "react";

import { XmldocNode } from "./XmldocNode";

import { PackageDoc } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocSeeAlsoProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocSeeAlso: React.StatelessComponent<XmldocSeeAlsoProps> = ({ data, pkg }) => {
    if (!data || !data.s)
        return null;
    return (
        <div>
            <h2>See Also</h2>
            {data.s.map((x, i) => <XmldocNode data={x} pkg={pkg} key={i}/>)}
        </div>
    );
}