import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router";
import ListItem from "material-ui/List/ListItem";

import { HashFilteredList, FilteredListItem } from "./HashFilteredList";
import { PackageEntityLink } from "./links";

import { PackageInjectedProps, withAutoPackage } from "./hoc";
import { simpleDeclaration } from "../fragments";
import { selectMany } from "../util";

interface RouteParams {
    ns: string;
}

export interface PackageNamespaceProps extends RouteComponentProps<RouteParams>, PackageInjectedProps {
}

const PackageNamespaceComponent: React.StatelessComponent<PackageNamespaceProps> = (props) => {
    const { pkg, pkgRequestKey, match: { params: { ns } } } = props;
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
        content:
        <PackageEntityLink {...pkgRequestKey} dnaid={x.i} key={x.i}>
            <ListItem><code>{simpleDeclaration(props, x, x.s)}</code></ListItem>
        </PackageEntityLink>
    }));

    return (
        <div>
            <h1><code>{ns}</code> Namespace</h1>

            <h2>Types</h2>
            <HashFilteredList items={typeList}/>
        </div>
    );
}

export const PackageNamespace = withAutoPackage(withRouter(PackageNamespaceComponent));
