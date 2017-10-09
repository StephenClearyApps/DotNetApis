import * as React from 'react';

import { XmldocNode } from ".";

import { PackageDoc } from "../../util";
import { IEntity, IParameter } from "../../structure";
import { parameterDeclaration } from "../../fragments";

interface ParameterProps {
    entity: IEntity;
    parameter: IParameter;
    pkg: PackageDoc;
}

export const Parameter: React.StatelessComponent<ParameterProps> = ({ entity, parameter, pkg }) => (
    <div>
        <pre className='highlight'>{parameterDeclaration(pkg, entity, parameter)}</pre>
        <XmldocNode data={parameter.x} pkg={pkg}/>
    </div>
);
