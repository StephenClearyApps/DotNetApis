import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router";
import ListItem from "material-ui/List/ListItem";

import { HashFilteredList, FilteredListItem } from "./HashFilteredList";
import { PackageEntityLink } from "./links";

import { PackageInjectedProps, withAutoPackage } from "./hoc";
import { packageFriendlyName, normalizePath, sortEntities, fileName } from "../util";
import { simpleDeclaration, attributeDeclaration } from "../fragments";

interface RouteParams {
    path: string;
}

export interface PackageFileProps extends RouteComponentProps<RouteParams>, PackageInjectedProps {
}

const PackageFileComponent: React.StatelessComponent<PackageFileProps> = (props) => {
    const { pkg, pkgRequestKey, match: { params: { path } } } = props;
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
            content:
                <PackageEntityLink {...pkgRequestKey} dnaid={x.i} key={x.i}>
                    <ListItem><code>{simpleDeclaration(props, x)}</code></ListItem>
                </PackageEntityLink>
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

export const PackageFile = withAutoPackage(withRouter(PackageFileComponent));
