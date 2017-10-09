import * as React from 'react';

import { XmldocBasic, XmldocRemarks, XmldocReturn, XmldocExceptions, XmldocExamples, XmldocSeeAlso, Parameters } from ".";

import { PackageDoc } from "../../util";
import { IMethodEntity } from "../../structure";
import { title, declarationLocation, declaration } from "../../fragments";

interface MethodProps {
    data: IMethodEntity;
    pkg: PackageDoc;
}

export const Method: React.StatelessComponent<MethodProps> = ({ data, pkg }) => (
    <div>
        <h1>{title(pkg, data)}</h1>

        <XmldocBasic data={data.x} pkg={pkg}/>

        <h2>Declaration</h2>
        <pre className='highlight'><span className='c'>// At {declarationLocation(pkg, data)}</span><br/>{declaration(pkg, data)}</pre>

        <XmldocRemarks data={data.x} pkg={pkg}/>
        <Parameters data={data} pkg={pkg}/>
        <XmldocReturn data={data.x} pkg={pkg}/>
        <XmldocExceptions data={data.x} pkg={pkg}/>
        <XmldocExamples data={data.x} pkg={pkg}/>
        <XmldocSeeAlso data={data.x} pkg={pkg}/>
    </div>
);
