import * as React from "react";

import { Xmldoc } from "./Xmldoc";

import { PackageDoc } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocBasicProps {
    data: IXmldoc;
    pkg: PackageDoc;
}

export const XmldocBasic: React.StatelessComponent<XmldocBasicProps> = ({ data, pkg }) => {
    if (!data || !data.b)
        return null;
    return <Xmldoc data={data.b} pkg={pkg}/>;
}