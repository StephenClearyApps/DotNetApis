import * as React from 'react';

import { Parameter } from "./Parameter";

import { PackageContext } from "../../util";
import { IParameterizedEntityBase } from "../../structure";

interface ParametersProps extends PackageContext {
    data: IParameterizedEntityBase;
}

export const Parameters: React.StatelessComponent<ParametersProps> = props => {
    const { data } = props;
    if (!data.p)
        return null;
    return (
        <div>
            <h2>Parameters</h2>
            {data.p.map((x, i) => <Parameter {...props} entity={data} parameter={x} key={i}/>)}
        </div>
    );
};
