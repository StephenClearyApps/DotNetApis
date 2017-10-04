import * as React from "react";
import { Link } from "react-router-dom";
import { FlatButton } from "material-ui";

import { LogMessages } from "./LogMessages";
import { State } from "../reducers";
import * as actions from "../actions";

export function Home(props: State & typeof actions)
{
    return (
    <div>
        <Link to="/doc">Load</Link>
        {props.packageDoc.logs["nito.asyncex/4.0.1/net45"] ? <LogMessages messages={props.packageDoc.logs["nito.asyncex/4.0.1/net45"]} currentTimestamp={props.time.timestamp} /> : null}
        <FlatButton onClick={() => props.DocActions.getDoc({ packageId: "FSharp.Reactive" })}>Click me!</FlatButton>
    </div>);
}