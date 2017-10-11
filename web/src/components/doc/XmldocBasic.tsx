import * as React from "react";

import { XmldocNode } from ".";

import { PackageContext } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocBasicProps extends PackageContext {
    data: IXmldoc;
}

export const XmldocBasic: React.StatelessComponent<XmldocBasicProps> = (props) => {
    const { data } = props;
    if (!data || !data.b)
        return null;
    return <XmldocNode {...props} data={data.b}/>;
}