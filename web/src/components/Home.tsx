import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { FlatButton, ListItem } from "material-ui";

import { HashFilteredGroupedList, FilteredListItemGroup } from "./HashFilteredGroupedList";

import { State } from "../reducers";
import { Actions } from "../actions";

export function Home(props: State & Actions & RouteComponentProps<{}>)
{
    const groups: FilteredListItemGroup[] = [
        {
            heading: "People",
            items: [
                {
                    content: <ListItem key="0">Bob</ListItem>,
                    search: "Bob"
                },
                {
                    content: <ListItem key="1">Mandyb</ListItem>,
                    search: "Mandyb"
                }
            ]
        },
        {
            heading: "Organizations",
            items: [
                {
                    content: <ListItem key="0">First</ListItem>,
                    search: "First"
                },
                {
                    content: <ListItem key="1">Second</ListItem>,
                    search: "Second"
                }
            ]
        }
    ];

    return (
    <div>
        <div><Link to="/pkg/Nito.AsyncEx">Load</Link></div>
        <HashFilteredGroupedList groups={groups} />
    </div>);
}

// <FlatButton onClick={() => props.DocActions.getDoc({ packageId: "FSharp.Reactive" })}>Click me!</FlatButton>
