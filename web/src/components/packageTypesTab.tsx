import * as React from "react";
import { Tab } from "material-ui";

import { HashFilteredList, FilteredListItem } from "./HashFilteredList";

import { IEntity } from "../structure";
import { PackageDoc } from "../util";

export function packageTypesTab(pkg: PackageDoc, types: IEntity[]) {
    const items : FilteredListItem[] = types.map(x => ({
        search: x.n,
        content: null
    }));
    return <Tab label="Types" value="types"><HashFilteredList items={items} hashPrefix="types" /></Tab>;
}
