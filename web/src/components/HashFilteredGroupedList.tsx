import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router";
import { parse, stringify } from "query-string";

import { FilteredListItem, FilteredListItemGroup, FilteredGroupedList } from "./FilteredGroupedList";
export { FilteredListItem, FilteredListItemGroup } from "./FilteredGroupedList";

export interface HashFilteredGroupedListProps {
    groups: FilteredListItemGroup[];
}

interface HashParams {
    filter: string;
}

const HashFilteredGroupedListComponent: React.StatelessComponent<HashFilteredGroupedListProps & RouteComponentProps<any>> = ({ groups, location, history }) => {
    const hashParams : HashParams = parse(location.hash);
    return <FilteredGroupedList groups={groups}
            filter={hashParams.filter}
            filterChanged={value => history.replace("#" + stringify({ ...hashParams, filter: value }))} />;
};

export const HashFilteredGroupedList = withRouter(HashFilteredGroupedListComponent);