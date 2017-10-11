import * as React from 'react';

import { XmldocBasic, XmldocRemarks, XmldocReturn, XmldocExceptions, XmldocExamples, XmldocSeeAlso, Parameters } from ".";

import { PackageContext } from "../../util";
import { IDelegateEntity } from "../../structure";
import { titleDeclaration, declarationLocation, declaration } from "../../fragments";

interface DelegateProps extends PackageContext {
    data: IDelegateEntity;
}

export const Delegate: React.StatelessComponent<DelegateProps> = props => {
    const { data } = props;
    return (
    <div>
        <h1>{titleDeclaration(props, data)}</h1>

        <XmldocBasic {...props} data={data.x}/>

        <h2>Declaration</h2>
        <pre className='highlight'><span className='c'>// At {declarationLocation(props, data)}</span><br/>{declaration(props, data)}</pre>

        <XmldocRemarks {...props} data={data.x}/>
        <Parameters {...props} data={data}/>
        <XmldocReturn {...props} data={data.x}/>
        <XmldocExceptions {...props} data={data.x}/>
        <XmldocExamples {...props} data={data.x}/>
        <XmldocSeeAlso {...props} data={data.x}/>
    </div>
    );
};