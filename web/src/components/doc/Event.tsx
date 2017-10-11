import * as React from 'react';

import { XmldocBasic, XmldocRemarks, XmldocReturn, XmldocExceptions, XmldocExamples, XmldocSeeAlso } from ".";

import { PackageContext } from "../../util";
import { IEventEntity } from "../../structure";
import { title, declarationLocation, declaration } from "../../fragments";

interface EventProps extends PackageContext {
    data: IEventEntity;
}

export const Event: React.StatelessComponent<EventProps> = props => {
    const { data } = props;
    return (
    <div>
        <h1>{title(props, data)}</h1>

        <XmldocBasic {...props} data={data.x}/>

        <h2>Declaration</h2>
        <pre className='highlight'><span className='c'>// At {declarationLocation(props, data)}</span><br/>{declaration(props, data)}</pre>

        <XmldocRemarks {...props} data={data.x}/>
        <XmldocReturn {...props} data={data.x}/>
        <XmldocExceptions {...props} data={data.x}/>
        <XmldocExamples {...props} data={data.x}/>
        <XmldocSeeAlso {...props} data={data.x}/>
    </div>
    );
};