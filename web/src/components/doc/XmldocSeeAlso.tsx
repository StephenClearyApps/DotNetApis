import * as React from "react";

import { XmldocNode } from ".";

import { PackageContext } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocSeeAlsoProps extends PackageContext {
    data?: IXmldoc;
}

export const XmldocSeeAlso: React.StatelessComponent<XmldocSeeAlsoProps> = (props) => {
    const { data } = props;
    if (!data || !data.s)
        return null;
    return (
        <div>
            <h2>See Also</h2>
            {data.s.map((x, i) => <XmldocNode {...props} data={x} key={i}/>)}
        </div>
    );
}