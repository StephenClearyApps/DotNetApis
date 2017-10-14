import * as React from "react";
import Tab from "material-ui/Tabs/Tab";
import ListItem from "material-ui/List/ListItem";

import { HashTabs } from "./HashTabs";
import { FilteredListItem, HashFilteredList } from "./HashFilteredList";
import { HashFilteredGroupedList, FilteredListItemGroup } from "./HashFilteredGroupedList";
import { PackageEntityLink, PackageNamespaceLink, PackageFileLink } from "./links";
import { PackageTile } from "./PackageTile";

import { State } from "../reducers";
import { Actions } from "../actions";
import { withAutoPackage, PackageInjectedProps } from "./hoc";
import { PackageContext } from "../util";
import { IEntity, IAssembly, IPackageDependency } from "../structure";
import { simpleDeclaration } from "../fragments";
import { packageEntityLink } from "../logic";

export interface PackageProps extends State, Actions {
}

function normalizePath(path: string): string {
    return path.replace(/\\/g, '/');
}

const PackageComponent: React.StatelessComponent<PackageProps & PackageInjectedProps> = props => {
    const { pkg, packageDoc, pkgRequestKey } = props;
    const types = pkg.l.map(x => x.t).reduce((a, b) => a.concat(b), []);
    types.sort((x, y) => {
        if (x.n < y.n)
            return -1;
        if (x.n > y.n)
            return 1;
        return 0;
    });
    const tabTypes = typesTab(props, types);
    const tabNamespaces = namespacesTab(props, types);
    const tabAssemblies = assembliesTab(props, pkg.l);
    const tabDependencies = dependenciesTab(props, pkg.e);

    const defaultTabValue = tabTypes ? "types" :
        tabNamespaces ? "namespaces" :
        tabAssemblies ? "assemblies" :
        tabDependencies ? "dependencies" : undefined;
    return (
    <HashTabs defaultTabValue={defaultTabValue}>
        {tabTypes}
        {tabNamespaces}
        {tabAssemblies}
        {tabDependencies}
    </HashTabs>);
}

export const Package = withAutoPackage(PackageComponent);

function typesTab(pkgContext: PackageContext, types: IEntity[]) {
    if (types.length === 0)
        return null;
    const items : FilteredListItem[] = types.map(x => ({
        search: x.n,
        content:
            <PackageEntityLink {...pkgContext.pkgRequestKey} dnaid={x.i} key={x.i}>
                <ListItem><code>{simpleDeclaration(pkgContext, x, (x as any).s)}</code></ListItem>
            </PackageEntityLink>
    }));
    return <Tab label="Types" value="types" key="types"><HashFilteredList items={items} hashPrefix="types" /></Tab>;
}

function namespacesTab(pkgContext: PackageContext, types: IEntity[]) {
    if (types.length === 0)
        return null;
    const namespaceMap : { [key: string]: 0 } = {};
    for (let t of types) {
        const ns = (t as any).s;
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
    return <Tab label="Namespaces" value="namespaces" key="namespaces"><HashFilteredList items={items} hashPrefix="namespaces"/></Tab>;
}

function assembliesTab(pkgContext: PackageContext, assemblies: IAssembly[]) {
    if (assemblies.length === 0)
        return null;
    const items : FilteredListItem[] = assemblies.map(x => normalizePath(x.p)).map(path => ({
        search: path,
        content:
            <PackageFileLink {...pkgContext.pkgRequestKey} path={path} key={path}>
                <ListItem><code>{path}</code></ListItem>
            </PackageFileLink>
    }));
    return <Tab label="Assemblies" value="assemblies" key="assemblies"><HashFilteredList items={items} hashPrefix="assemblies" /></Tab>;
}

function dependenciesTab(pkgContext: PackageContext, dependencies: IPackageDependency[]) {
    if (dependencies.length === 0)
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
    return <Tab label="Dependencies" value="dependencies" key="dependencies"><HashFilteredGroupedList groups={groups} hashPrefix="dependencies" /></Tab>;
}