import * as React from 'react';
import { ListItem } from 'material-ui';

import { XmldocBasic, XmldocRemarks, XmldocExamples, XmldocSeeAlso } from ".";
import { HashFilteredGroupedList, FilteredListItemGroup } from "../HashFilteredGroupedList";
import { PackageEntityLink } from '../links';

import { PackageContext } from "../../util";
import { ITypeEntity, IEntity } from "../../structure";
import { title, declarationLocation, declaration, simpleDeclaration } from "../../fragments";

interface TypeProps extends PackageContext {
    data: ITypeEntity;
}

export const Type: React.StatelessComponent<TypeProps> = (props) => {
    const { data } = props;
    const groups = [
        memberGrouping("Lifetime", data.e.l, props),
        memberGrouping("Static", data.e.s, props),
        memberGrouping("Instance", data.e.i, props),
        // TODO: ("Protected", data.e.d, pkg),
        memberGrouping("Nested types", data.e.t, props)
    ].filter(x => x !== null);
    return (
        <div>
            <h1>{title(props, data)}</h1>

            <XmldocBasic {...props} data={data.x}/>

            <h2>Declaration</h2>
            <pre className='highlight'><span className='c'>// At {declarationLocation(props, data)}</span><br/>{declaration(props, data)}</pre>

            <XmldocRemarks {...props} data={data.x}/>

            <h2>Members</h2>
            <HashFilteredGroupedList groups={groups} />

            <XmldocExamples {...props} data={data.x}/>
            <XmldocSeeAlso {...props} data={data.x}/>
        </div>
    );
};

function memberGrouping(name: string, items: IEntity[], pkgContext: PackageContext): FilteredListItemGroup {
    if (!items || items.length === 0)
        return null;
    return {
        heading: name,
        items: items.map(x => ({
                search: x.n,
                content:
                    <PackageEntityLink key={x.i} {...pkgContext.pkgRequestKey} dnaid={x.i}>
                        <ListItem><code>{simpleDeclaration(pkgContext, x)}</code></ListItem>
                    </PackageEntityLink>
        }))
    };
}
