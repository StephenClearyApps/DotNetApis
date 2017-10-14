import * as React from "react";
import { RouteComponentProps } from "react-router";

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
