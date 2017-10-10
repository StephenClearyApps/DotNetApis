import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router";

import { FilteredListItem, FilteredListItemGroup, FilteredGroupedList } from "./FilteredGroupedList";
export { FilteredListItem, FilteredListItemGroup } from "./FilteredGroupedList";

import { HashFilter } from "../logic";

export interface HashFilteredGroupedListProps {
    groups: FilteredListItemGroup[];
}

const HashFilteredGroupedListComponent: React.StatelessComponent<HashFilteredGroupedListProps & RouteComponentProps<any>> = ({ groups, location, history }) => {
    const hash = new HashFilter(location, history);
    return <FilteredGroupedList groups={groups}
            filter={hash.filter}
            filterChanged={value => hash.filter = value} />;
};

export const HashFilteredGroupedList = withRouter(HashFilteredGroupedListComponent);