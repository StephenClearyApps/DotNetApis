import * as React from "react";
import { Link } from "react-router-dom";
import { FlatButton } from "material-ui";

import { State } from "../reducers";
import { Actions } from "../actions";

export function Home(props: State & Actions)
{
    return (
    <div>
        <Link to="/pkg/Nito.AsyncEx">Load</Link>
    </div>);
}

// <FlatButton onClick={() => props.DocActions.getDoc({ packageId: "FSharp.Reactive" })}>Click me!</FlatButton>
