import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router";
import { parse, stringify } from "query-string";

import { FilteredListItem, FilteredList } from "./FilteredList";
export { FilteredListItem } from "./FilteredList";

export interface HashFilteredListProps {
    items: FilteredListItem[];
}

interface HashParams {
    filter: string;
}

const HashFilteredListComponent: React.StatelessComponent<HashFilteredListProps & RouteComponentProps<any>> = ({ items, location, history }) => {
    const hashParams : HashParams = parse(location.hash);
    return <FilteredList items={items}
            filter={hashParams.filter}
            filterChanged={value => history.replace("#" + stringify({ ...hashParams, filter: value }))} />;
};

export const HashFilteredList = withRouter(HashFilteredListComponent);