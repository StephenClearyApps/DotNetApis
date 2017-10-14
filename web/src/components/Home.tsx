import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import FlatButton from "material-ui/FlatButton";
import ListItem from "material-ui/List/ListItem";

import { HashFilteredGroupedList, FilteredListItemGroup } from "./HashFilteredGroupedList";
import { FrontPagePackages } from "./FrontPagePackages";

import { State } from "../reducers";
import { Actions } from "../actions";

export function Home(props: State & Actions & RouteComponentProps<{}>)
{
    return (
    <div>
        <FrontPagePackages/>
    </div>);
}
