import * as React from 'react';

import { XmldocBasic, XmldocRemarks, XmldocExamples, XmldocSeeAlso } from ".";
import { HashFilteredGroupedList, FilteredListItemGroup } from "../HashFilteredGroupedList";
import { EntityListItem } from '../EntityListItem';

import { PackageContext } from "../../util";
import { ITypeEntity, IEntity } from "../../structure";
import { titleDeclaration, declarationLocation, declaration } from "../../fragments";

interface TypeProps extends PackageContext {
    data: ITypeEntity;
}

export const Type: React.StatelessComponent<TypeProps> = (props) => {
    const { data } = props;
    const groups = [
        memberGrouping("Lifetime", data.e ? data.e.l : [], props),
        memberGrouping("Static", data.e ? data.e.s : [], props),
        memberGrouping("Instance", data.e ? data.e.i : [], props),
        // TODO: ("Protected", data.e ? data.e.d : [], pkg),
        memberGrouping("Nested types", data.e ? data.e.t : [], props)
    ].filter(notNull);
    return (
        <div>
            <h1>{titleDeclaration(props, data)}</h1>

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

function memberGrouping(name: string, items: IEntity[] | undefined, pkgContext: PackageContext): FilteredListItemGroup | null {
    if (!items || items.length === 0)
        return null;
    return {
        heading: name,
        items: items.map(x => ({
                search: x.n!,
                content: <EntityListItem key={x.i} pkgContext={pkgContext} entity={x} />
        }))
    };
}

function notNull<T>(value: T | null): value is T {
    return value !== null;
}