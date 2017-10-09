import * as React from 'react';

import { Parameter } from "./Parameter";

import { PackageDoc } from "../../util";
import { IParameterizedEntityBase } from "../../structure";

interface ParametersProps {
    data: IParameterizedEntityBase;
    pkg: PackageDoc;
}

export const Parameters: React.StatelessComponent<ParametersProps> = ({ data, pkg }) => {
    if (!data.p)
        return null;
    return (
        <div>
            <h2>Parameters</h2>
            {data.p.map((x, i) => <Parameter entity={data} parameter={x} key={i} pkg={pkg}/>)}
        </div>
    );
};
