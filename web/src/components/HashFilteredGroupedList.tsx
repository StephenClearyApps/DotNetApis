import * as React from "react";

import { RouteComponentProps, createRouterProps } from "./hoc";
import { FilteredListItemGroup, FilteredGroupedList } from "./FilteredGroupedList";
export { FilteredListItem, FilteredListItemGroup } from "./FilteredGroupedList";

import { HashSettings } from "../logic";

export interface HashFilteredGroupedListProps {
    hashPrefix?: string;
    groups: FilteredListItemGroup[];
}

const HashFilteredGroupedListComponent: React.StatelessComponent<HashFilteredGroupedListProps & RouteComponentProps<{}>> =
({ groups, location, history, hashPrefix }) => {
    const hash = new HashSettings(location, history, hashPrefix);
    return <FilteredGroupedList groups={groups}
            filter={hash.getSetting("filter")}
            filterChanged={value => hash.setSetting("filter", value)} />;
};

export const HashFilteredGroupedList = createRouterProps()(HashFilteredGroupedListComponent);