import * as React from "react";

import { XmldocNode } from ".";

import { PackageContext } from "../../util";
import { IXmldoc } from "../../structure";

interface XmldocExampesProps extends PackageContext {
    data: IXmldoc;
}

export const XmldocExamples: React.StatelessComponent<XmldocExampesProps> = (props) => {
    const { data } = props;
    if (!data || !data.e)
        return null;
    return (
        <div>
            <h2>Examples</h2>
            {data.e.map((x, i) => <XmldocNode {...props} data={x} key={i}/>)}
        </div>
    );
}