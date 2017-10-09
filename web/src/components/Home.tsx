import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { FlatButton, ListItem } from "material-ui";
import { parse, stringify } from "query-string";

import { FilteredList, FilteredListItem } from "./FilteredList";

import { State } from "../reducers";
import { Actions } from "../actions";

interface HashParams {
    homeFilter: string;
}

export function Home(props: State & Actions & RouteComponentProps<{}>)
{
    const hashParams : HashParams = parse(props.location.hash);

    const items: FilteredListItem[] = [
        {
            content: <ListItem key="0">Bob</ListItem>,
            search: "Bob"
        },
        {
            content: <ListItem key="1">Mandyb</ListItem>,
            search: "Mandyb"
        },
    ];

    return (
    <div>
        <div><Link to="/pkg/Nito.AsyncEx">Load</Link></div>
        <FilteredList items={items}
            filter={hashParams.homeFilter}
            filterChanged={value => props.history.replace("#" + stringify({ ...hashParams, homeFilter: value }))} />
    </div>);
}

// <FlatButton onClick={() => props.DocActions.getDoc({ packageId: "FSharp.Reactive" })}>Click me!</FlatButton>
