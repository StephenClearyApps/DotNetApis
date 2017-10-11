import * as React from 'react';
import { ListItem } from 'material-ui';

import { XmldocBasic, XmldocRemarks, XmldocExamples, XmldocSeeAlso } from ".";
import { HashFilteredGroupedList, FilteredListItemGroup } from "../HashFilteredGroupedList";
import { PackageEntityLink } from '../links';

import { PackageDoc } from "../../util";
import { ITypeEntity, IEntity } from "../../structure";
import { title, declarationLocation, declaration, simpleDeclaration } from "../../fragments";

interface TypeProps {
    data: ITypeEntity;
    pkg: PackageDoc;
}

export const Type: React.StatelessComponent<TypeProps> = ({ data, pkg }) => {
    const groups = [
        memberGrouping("Lifetime", data.e.l, pkg),
        memberGrouping("Static", data.e.s, pkg),
        memberGrouping("Instance", data.e.i, pkg),
        // TODO: ("Protected", data.e.d, pkg),
        memberGrouping("Nested types", data.e.t, pkg)
    ].filter(x => x !== null);
    return (
        <div>
            <h1>{title(pkg, data)}</h1>

            <XmldocBasic data={data.x} pkg={pkg}/>

            <h2>Declaration</h2>
            <pre className='highlight'><span className='c'>// At {declarationLocation(pkg, data)}</span><br/>{declaration(pkg, data)}</pre>

            <XmldocRemarks data={data.x} pkg={pkg}/>

            <h2>Members</h2>
            <HashFilteredGroupedList groups={groups} />

            <XmldocExamples data={data.x} pkg={pkg}/>
            <XmldocSeeAlso data={data.x} pkg={pkg}/>
        </div>
    );
};

function memberGrouping(name: string, items: IEntity[], pkg: PackageDoc): FilteredListItemGroup {
    if (!items || items.length === 0)
        return null;
    return {
        heading: name,
        items: items.map(x => ({
                search: x.n,
                content:
                    <PackageEntityLink key={x.i} {...pkg.getPackageKey()} dnaid={x.i}>
                        <ListItem><code>{simpleDeclaration(pkg, x)}</code></ListItem>
                    </PackageEntityLink>
        }))
    };
}
