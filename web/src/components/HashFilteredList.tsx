import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router";

import { FilteredListItem, FilteredList } from "./FilteredList";
export { FilteredListItem } from "./FilteredList";

import { HashSettings } from "../logic";

export interface HashFilteredListProps {
    hashPrefix?: string;
    items: FilteredListItem[];
}

const HashFilteredListComponent: React.StatelessComponent<HashFilteredListProps & RouteComponentProps<any>> =
({ items, location, history, hashPrefix }) => {
    const hash = new HashSettings(location, history, hashPrefix);
    return <FilteredList items={items}
            filter={hash.getSetting("filter")}
            filterChanged={value => hash.setSetting("filter", value)} />;
};

export const HashFilteredList = withRouter(HashFilteredListComponent);