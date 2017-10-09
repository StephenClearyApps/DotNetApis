import * as React from 'react';

import { XmldocBasic, XmldocRemarks, XmldocExamples, XmldocSeeAlso } from ".";

import { PackageDoc } from "../../util";
import { IEnumEntity } from "../../structure";
import { title, declarationLocation, declaration } from "../../fragments";

interface EnumProps {
    data: IEnumEntity;
    pkg: PackageDoc;
}

export const Enum: React.StatelessComponent<EnumProps> = ({ data, pkg }) => (
    <div>
        <h1>{title(pkg, data)}</h1>

        <XmldocBasic data={data.x} pkg={pkg}/>

        <h2>Declaration</h2>
        <pre className='highlight'><span className='c'>// At {declarationLocation(pkg, data)}</span><br/>{declaration(pkg, data)}</pre>

        <XmldocRemarks data={data.x} pkg={pkg}/>

        <h2>Members</h2>
        <div>TODO</div>

        <XmldocExamples data={data.x} pkg={pkg}/>
        <XmldocSeeAlso data={data.x} pkg={pkg}/>
    </div>
);
