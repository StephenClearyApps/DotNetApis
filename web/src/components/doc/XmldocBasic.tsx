import * as React from "react";

import { XmldocNode } from "./XmldocNode";

import { PackageDoc } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocBasicProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocBasic: React.StatelessComponent<XmldocBasicProps> = ({ data, pkg }) => {
    if (!data || !data.b)
        return null;
    return <XmldocNode data={data.b} pkg={pkg}/>;
}