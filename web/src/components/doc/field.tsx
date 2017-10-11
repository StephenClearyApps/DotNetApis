import * as React from 'react';

import { XmldocBasic, XmldocRemarks, XmldocExamples, XmldocSeeAlso } from ".";

import { PackageContext } from "../../util";
import { IFieldEntity } from "../../structure";
import { titleDeclaration, declarationLocation, declaration } from "../../fragments";

interface FieldProps extends PackageContext {
    data: IFieldEntity;
}

export const Field: React.StatelessComponent<FieldProps> = props => {
    const { data } = props;
    return(
    <div>
        <h1>{titleDeclaration(props, data)}</h1>

        <XmldocBasic {...props} data={data.x}/>

        <h2>Declaration</h2>
        <pre className='highlight'><span className='c'>// At {declarationLocation(props, data)}</span><br/>{declaration(props, data)}</pre>

        <XmldocRemarks {...props} data={data.x}/>
        <XmldocExamples {...props} data={data.x}/>
        <XmldocSeeAlso {...props} data={data.x}/>
    </div>
    );
};