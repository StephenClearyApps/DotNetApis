import * as React from 'react';

import { XmldocNode } from ".";

import { PackageContext } from "../../util";
import { IEntity, IParameter } from "../../structure";
import { parameterDeclaration } from "../../fragments";

interface ParameterProps extends PackageContext {
    entity: IEntity;
    parameter: IParameter;
}

export const Parameter: React.StatelessComponent<ParameterProps> = props => {
    const { entity, parameter } = props;
    return (
    <div>
        <pre className='highlight'>{parameterDeclaration(props, entity, parameter)}</pre>
        <XmldocNode {...props} data={parameter.x}/>
    </div>
    );
};
