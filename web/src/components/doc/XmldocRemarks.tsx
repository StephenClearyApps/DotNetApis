import * as React from "react";

import { XmldocNode } from ".";

import { PackageContext } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocRemarksProps extends PackageContext {
    data: IXmldoc;
}

export const XmldocRemarks: React.StatelessComponent<XmldocRemarksProps> = (props) => {
    const { data } = props;
    if (!data || !data.m)
        return null;
    return (
        <div>
            <h2>Remarks</h2>
            <XmldocNode {...props} data={data.m}/>)
        </div>
    );
}