import * as React from "react";
import IconButton from '@material-ui/core/IconButton';
import Toolbar from '@material-ui/core/Toolbar';
import ToolbarGroup from '@material-ui/core/Toolbar';
import TextField from '@material-ui/core/TextField';
import List from '@material-ui/core/List';
import Tooltip from "@material-ui/core/Tooltip";
import ClearIcon from '@material-ui/icons/Clear';

export interface FilteredListItem {
    /** The content to display */
    content: React.ReactElement;
    /** The search text to apply the filter to */
    search: string;
}

export interface FilteredListProps {
    /** The current filter, treated as a case-insensitive regular expression */
    filter?: string;
    filterChanged: (filter: string | undefined) => void;
    items: FilteredListItem[];
}

function generateRegex(filter: string | undefined) {
    try { return new RegExp(filter || "", "i"); }
    catch { return new RegExp("", "i"); }
}

export const FilteredList: React.StatelessComponent<FilteredListProps> = ({ filter, filterChanged, items }) => {
    if (filter === "")
        filter = undefined;
    const regex = generateRegex(filter);
    const filteredItems = items.filter(item => regex.test(item.search)).map(x => x.content);
    const fullCount = items.length;
    const filteredCount = filteredItems.length;
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
            <List>
                {filteredItems}
            </List>
        </div>
    );
};