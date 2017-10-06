import * as React from "react";
import { Route } from "react-router-dom";

import { Home } from "./Home";
import { Package } from "./Package";
import { State } from "../reducers";
import { Actions } from "../actions";

export function Main(props: State & Actions)
{
    return (
    <div>
        <Route exact path="/" render={() => <Home {...props} />} />
        <Route path="/pkg/:packageId/:packageVersion/:packageTarget" render={(routeProps) => <Package {...props} {...routeProps}/>}/>
    </div>);
}