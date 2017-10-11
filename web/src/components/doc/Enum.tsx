import * as React from 'react';

import { XmldocBasic, XmldocRemarks, XmldocExamples, XmldocSeeAlso } from ".";

import { PackageContext } from "../../util";
import { IEnumEntity } from "../../structure";
import { title, declarationLocation, declaration } from "../../fragments";

interface EnumProps extends PackageContext {
    data: IEnumEntity;
}

export const Enum: React.StatelessComponent<EnumProps> = props => {
    const { data } = props;
    return (
    <div>
        <h1>{title(props, data)}</h1>

        <XmldocBasic {...props} data={data.x}/>

        <h2>Declaration</h2>
        <pre className='highlight'><span className='c'>// At {declarationLocation(props, data)}</span><br/>{declaration(props, data)}</pre>

        <XmldocRemarks {...props} data={data.x}/>

        <h2>Members</h2>
        <div>TODO</div>

        <XmldocExamples {...props} data={data.x}/>
        <XmldocSeeAlso {...props} data={data.x}/>
    </div>
    );
};