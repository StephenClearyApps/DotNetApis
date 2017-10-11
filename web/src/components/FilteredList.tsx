import * as React from "react";
import IconButton from "material-ui/IconButton";
import Toolbar from "material-ui/Toolbar";
import ToolbarGroup from "material-ui/Toolbar/ToolbarGroup";
import TextField from "material-ui/TextField";
import List from "material-ui/List";
import ListItem from "material-ui/List/ListItem";

export interface FilteredListItem {
    /** The content to display */
    content: React.ReactElement<ListItem>;
    /** The search text to apply the filter to */
    search: string;
}

export interface FilteredListProps {
    /** The current filter, treated as a case-insensitive regular expression */
    filter: string;
    filterChanged: (filter: string | undefined) => void;
    items: FilteredListItem[];
}

function generateRegex(filter: string) {
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
    const clearFilterButton = !filter ? null : <IconButton tooltip="Clear filter" onClick={() => filterChanged(undefined)} iconClassName="material-icons">clear</IconButton>;

    return (
        <div>
            <Toolbar>
                <ToolbarGroup style={{flex: "auto"}}>
                    <TextField hintText="Filter" value={filter || ""} onChange={(e, newValue) => filterChanged(newValue ? newValue : undefined)} style={{flex: "auto"}}/>
                </ToolbarGroup>
                <ToolbarGroup lastChild={true} style={{marginLeft: "24px"}}>
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