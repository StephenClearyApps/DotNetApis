import * as React from "react";
import { Route } from "react-router-dom";

import { Home } from "./Home";
import { State } from "../reducers";
import * as actions from "../actions";

export function Main(props: State & typeof actions)
{
    return (
    <div>
        <Route exact path="/" render={() => <Home {...props} />} />
    </div>);
}