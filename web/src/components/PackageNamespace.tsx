import * as React from "react";

import { HashFilteredList, FilteredListItem } from "./HashFilteredList";
import { EntityListItem } from './EntityListItem';

import { PackageInjectedProps, withPackage, RouteComponentProps, createRouterProps } from "./hoc";
import { selectMany } from "../util";
import { State } from "../reducers";
import { Actions } from "../actions";

interface RouteParams {
    ns: string;
}

export type PackageNamespaceProps = State & Actions;
const PackageNamespaceComponent: React.StatelessComponent<PackageNamespaceProps & RouteComponentProps<RouteParams> & PackageInjectedProps> = (props) => {
    const { pkg, match: { params: { ns } } } = props;
    const types = selectMany(pkg.l, x => x.t).filter(x => x.s === ns);
    types.sort((x, y) => {
        if (x.n < y.n)
            return -1;
        if (x.n > y.n)
            return 1;
        return 0;
    });

    const typeList = types.map(x => ({
        search: x.n,
        content: <EntityListItem key={x.i} pkgContext={props} entity={x}/>
    }));

    return (
        <div>
            <h1><code>{ns}</code> Namespace</h1>

            <h2>Types</h2>
            <HashFilteredList items={typeList}/>
        </div>
    );
}

export const PackageNamespace = withPackage(createRouterProps<RouteParams>()(PackageNamespaceComponent));
