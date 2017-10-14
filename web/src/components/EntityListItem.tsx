import * as React from "react";
import ListItem from 'material-ui/List/ListItem';

import { PackageEntityLink } from "./links";

import { PackageContext } from "../util";
import { IEntity } from "../structure";
import { simpleDeclaration } from "../fragments";

export interface EntityListItemProps {
    pkgContext: PackageContext;
    entity: IEntity;
}

export const EntityListItem: React.StatelessComponent<EntityListItemProps> = ({ pkgContext, entity }) => {
    return (
        <PackageEntityLink key={entity.i} {...pkgContext.pkgRequestKey} dnaid={entity.i}>
            <ListItem><code>{simpleDeclaration(pkgContext, entity)}</code></ListItem>
        </PackageEntityLink>
    );
}
