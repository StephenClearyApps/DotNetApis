import * as React from "react";

import { XmldocNode } from ".";

import { PackageContext } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocReturnProps extends PackageContext {
    data: IXmldoc;
}

export const XmldocReturn: React.StatelessComponent<XmldocReturnProps> = (props) => {
    const { data } = props;
    if (!data || !data.r)
        return null;
    return (
        <div>
            <h2>Return Value</h2>
            <XmldocNode {...props} data={data.r}/>
        </div>
    );
}