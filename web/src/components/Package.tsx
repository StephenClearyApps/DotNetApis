import * as React from "react";
import ListItem from '@material-ui/core/ListItem';
import Tab from '@material-ui/core/Tab';

import { HashTabs } from "./HashTabs";
import { FilteredListItem, HashFilteredList } from "./HashFilteredList";
import { HashFilteredGroupedList, FilteredListItemGroup } from "./HashFilteredGroupedList";
import { PackageNamespaceLink, PackageFileLink } from "./links";
import { PackageTile } from "./PackageTile";
import { EntityListItem } from './EntityListItem';

import { State } from "../reducers";
import { Actions } from "../actions";
import { withPackage, PackageInjectedProps } from "./hoc";
import { PackageContext, normalizePath, sortEntities, selectMany } from "../util";
import { IAssembly, IPackageDependency, IPackageEntity } from "../structure";

export type PackageProps = State & Actions;
const PackageComponent: React.StatelessComponent<PackageProps & PackageInjectedProps> = props => {
    const { pkg } = props;
    const types = selectMany(pkg.l, x => x.t);
    sortEntities(types);
    const typesContent = typesTabContent(props, types);
    const namespacesContent = namespacesTabContent(props, types);
    const assembliesContent = assembliesTabContent(props, pkg.l);
    const dependenciesContent = dependenciesTabContent(props, pkg.e);

    const defaultTabValue = typesContent ? "types" :
        namespacesContent ? "namespaces" :
        assembliesContent ? "assemblies" :
        dependenciesContent ? "dependencies" : undefined;
    return (
    <HashTabs defaultTabValue={defaultTabValue} content={{
        "types": typesContent,
        "namespaces": namespacesContent,
        "assemblies": assembliesContent,
        "dependencies": dependenciesContent
    }}>
        <Tab label="Types" value="types" key="types"/>
        <Tab label="Namespaces" value="namespaces" key="namespaces"/>
        <Tab label="Assemblies" value="assemblies" key="assemblies"/>
        <Tab label="Dependencies" value="dependencies" key="dependencies"/>
    </HashTabs>);
}

export const Package = withPackage(PackageComponent);

function typesTabContent(pkgContext: PackageContext, types: IPackageEntity[]) {
    if (types.length === 0)
        return null;
    const items : FilteredListItem[] = types.map(x => ({
        search: x.n!,
        content: <EntityListItem key={x.i} pkgContext={pkgContext} entity={x}/>
    }));
    return <HashFilteredList items={items} hashPrefix="types" />;
}

function namespacesTabContent(pkgContext: PackageContext, types: IPackageEntity[]) {
    if (types.length === 0)
        return null;
    const namespaceMap : { [key: string]: 0 } = {};
    for (let t of types) {
        const ns = t.s;
        if (ns)
            namespaceMap[ns] = 0;
    }
    const namespaces = Object.keys(namespaceMap);
    namespaces.sort();
    const items: FilteredListItem[] = Object.keys(namespaceMap).map(x => ({
        search: x,
        content:
            <PackageNamespaceLink {...pkgContext.pkgRequestKey} ns={x} key={x}>
                <ListItem><code>{x}</code></ListItem>
            </PackageNamespaceLink>
    }));
    return <HashFilteredList items={items} hashPrefix="namespaces"/>;
}

function assembliesTabContent(pkgContext: PackageContext, assemblies: IAssembly[] | undefined) {
    if (!assemblies || assemblies.length === 0)
        return null;
    const items : FilteredListItem[] = assemblies.map(x => normalizePath(x.p)).map(path => ({
        search: path,
        content:
            <PackageFileLink {...pkgContext.pkgRequestKey} path={path} key={path}>
                <ListItem><code>{path}</code></ListItem>
            </PackageFileLink>
    }));
    return <HashFilteredList items={items} hashPrefix="assemblies" />;
}

function dependenciesTabContent(pkgContext: PackageContext, dependencies: IPackageDependency[] | undefined) {
    if (!dependencies || dependencies.length === 0)
        return null;
    const groups: FilteredListItemGroup[] = [
        {
            heading: "Direct Dependencies",
            items: dependencies.filter(x => x.q).map(x => ({
                search: x.i,
                content: <PackageTile key={x.i} packageId={x.i} packageVersion={x.v} targetFramework={pkgContext.pkg.t} title={x.t} description={x.d} iconUrl={x.c}/>
            }))
        },
        {
            heading: "Indirect Dependencies",
            items: dependencies.filter(x => !x.q).map(x => ({
                search: x.i,
                content: <PackageTile key={x.i} packageId={x.i} packageVersion={x.v} targetFramework={pkgContext.pkg.t} title={x.t} description={x.d} iconUrl={x.c}/>
            }))
        }
    ];
    return <HashFilteredGroupedList groups={groups} hashPrefix="dependencies" />;
}