import * as React from "react";
import { Route } from "react-router-dom";

import { Home } from "./Home";
import { State } from "../reducers";
import { Actions } from "../actions";

export function Main(props: State & Actions)
{
    return (
    <div>
        <Route exact path="/" render={() => <Home {...props} />} />
    </div>);
}