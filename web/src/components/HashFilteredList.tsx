import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router";

import { FilteredListItem, FilteredList } from "./FilteredList";
export { FilteredListItem } from "./FilteredList";

import { HashFilter } from "../logic";

export interface HashFilteredListProps {
    items: FilteredListItem[];
}

const HashFilteredListComponent: React.StatelessComponent<HashFilteredListProps & RouteComponentProps<any>> = ({ items, location, history }) => {
    const hash = new HashFilter(location, history);
    return <FilteredList items={items}
            filter={hash.filter}
            filterChanged={value => hash.filter = value} />;
};

export const HashFilteredList = withRouter(HashFilteredListComponent);