import * as React from "react";
import { Link } from "react-router-dom";
import { FlatButton } from "material-ui";

import { LogMessages } from "./LogMessages";
import { State } from "../reducers";
import { Actions } from "../actions";

export function Home(props: State & Actions)
{
    return (
    <div>
        <Link to="/pkg/Nito.AsyncEx/4.0.1/net45">Load</Link>
        {props.packageDoc.logs["nito.asyncex/4.0.1/net45"] ? <LogMessages messages={props.packageDoc.logs["nito.asyncex/4.0.1/net45"]} currentTimestamp={props.time.timestamp} /> : null}
        <FlatButton onClick={() => props.DocActions.getDoc({ packageId: "FSharp.Reactive" })}>Click me!</FlatButton>
    </div>);
}