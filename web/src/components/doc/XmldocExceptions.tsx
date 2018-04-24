import * as React from "react";

import { XmldocNode } from ".";

import { PackageContext } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocExceptionsProps extends PackageContext {
    data?: IXmldoc;
}

export const XmldocExceptions: React.StatelessComponent<XmldocExceptionsProps> = (props) => {
    const { data } = props;
    if (!data || !data.x)
        return null;
    return (
        <div>
            <h2>Exceptions</h2>
            {data.x.map((x, i) => <XmldocNode {...props} data={x} key={i}/>)}
        </div>
    );
}