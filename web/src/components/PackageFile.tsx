import * as React from "react";

import { HashFilteredList, FilteredListItem } from "./HashFilteredList";
import { EntityListItem } from './EntityListItem';

import { PackageInjectedProps, withPackage, RouteComponentProps, createRouterProps } from "./hoc";
import { packageFriendlyName, normalizePath, sortEntities, fileName } from "../util";
import { attributeDeclaration } from "../fragments";
import { State } from "../reducers";
import { Actions } from "../actions";

interface RouteParams {
    path: string;
}

export type PackageFileProps = State & Actions;
const PackageFileComponent: React.StatelessComponent<PackageFileProps & RouteComponentProps<RouteParams> & PackageInjectedProps> = (props) => {
    const { pkg, match: { params: { path } } } = props;
    // TODO: better error message
    if (!pkg.l)
        return <div>Sorry; file {path} was not found in package {packageFriendlyName(pkg.getPackageKey())}.</div>;
    const file = pkg.l.find(x => normalizePath(x.p) === path);
    if (!file)
        return <div>Sorry; file {path} was not found in package {packageFriendlyName(pkg.getPackageKey())}.</div>;
    
    let typeList: FilteredListItem[] = [];
    if (file.t) {
        const types = file.t.slice();
        sortEntities(types);

        typeList = types.map(x => ({
            search: x.n,
            content: <EntityListItem key={x.i} pkgContext={props} entity={x} />
        }));
    }

    return (
        <div>
            <h1>{fileName(path)}</h1>

            <p>Full name: <code>{file.n}</code></p>

            <p>Location: <code>{path}</code></p>

            <p>Size: <code>{file.s}</code></p>

            {file.b ?
                <div>
                    <h2>Attributes</h2>
                    <pre className='highlight'>{attributeDeclaration(props, file.b)}</pre>
                </div> : null}

            <h2>Types</h2>
            <HashFilteredList items={typeList}/>
        </div>
    );
}

export const PackageFile = withPackage(createRouterProps<RouteParams>()(PackageFileComponent));
