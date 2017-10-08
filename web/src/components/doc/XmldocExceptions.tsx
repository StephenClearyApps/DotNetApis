import * as React from "react";

import { Xmldoc } from "./Xmldoc";

import { PackageDoc } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocExceptionsProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocExceptions: React.StatelessComponent<XmldocExceptionsProps> = ({ data, pkg }) => {
    if (!data || !data.x)
        return null;
    return (
        <div>
            <h2>Exceptions</h2>
            {data.x.map((x, i) => <Xmldoc data={x} pkg={pkg} key={i}/>)}
        </div>
    );
}