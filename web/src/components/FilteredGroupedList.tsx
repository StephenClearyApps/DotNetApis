import * as React from 'react';
import IconButton from '@material-ui/core/IconButton';
import Toolbar from '@material-ui/core/Toolbar';
import ToolbarGroup from '@material-ui/core/Toolbar';
import TextField from '@material-ui/core/TextField';
import List from '@material-ui/core/List';
import Subheader from "@material-ui/core/ListSubheader";
import Divider from '@material-ui/core/Divider';
import Tooltip from "@material-ui/core/Tooltip";
import ClearIcon from '@material-ui/icons/Clear';

import { FilteredListItem } from "./FilteredList";
import { join } from '../fragments/partial/util';
export { FilteredListItem } from "./FilteredList";

export interface FilteredListItemGroup {
    /** The heading content */
    heading: string;
    items: FilteredListItem[];
}

export interface FilteredGroupedListProps {
    /** The current filter, treated as a case-insensitive regular expression */
    filter?: string;
    filterChanged: (filter: string | undefined) => void;
    groups: FilteredListItemGroup[];
}

function generateRegex(filter: string | undefined) {
    try { return new RegExp(filter || "", "i"); }
    catch { return new RegExp("", "i"); }
}

function headingList(group: FilteredListItemGroup) {
    if (group.items.length === 0)
        return null;
    return [
        <List key={group.heading}>
            <Subheader>{group.heading}</Subheader>
            {group.items.map(x => x.content)}
        </List>
    ];
}

export const FilteredGroupedList: React.StatelessComponent<FilteredGroupedListProps> = ({ filter, filterChanged, groups }) => {
    if (filter === "")
        filter = undefined;
    const regex = generateRegex(filter);
    const filteredGroups = groups.map(x => ({ heading: x.heading, items: x.items.filter(item => regex.test(item.search)) }));
    const fullCount = groups.reduce((sum, value) => sum + value.items.length, 0);
    const filteredCount = filteredGroups.reduce((sum, value) => sum + value.items.length, 0);
    const filteredMessage = !filter ? null : <span>{filteredCount} of {fullCount}</span>;
    const clearFilterButton = !filter ? null : <Tooltip title="Clear filter"><IconButton onClick={() => filterChanged(undefined)}><ClearIcon/></IconButton></Tooltip>;

    return (
        <div>
            <Toolbar>
                <ToolbarGroup style={{flex: "auto"}}>
                    <TextField placeholder="Filter" value={filter || ""} onChange={(e) => filterChanged(e.target.value ? e.target.value : undefined)} style={{flex: "auto"}}/>
                </ToolbarGroup>
                <ToolbarGroup style={{marginLeft: "24px"}}>
                    {filteredMessage}
                    {clearFilterButton}
                </ToolbarGroup>
            </Toolbar>
            {join(filteredGroups.map(x => headingList(x)).filter(x => x !== null), [<Divider/>])}
        </div>
    );
};