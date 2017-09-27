import * as React from "react";

import { LogMessages } from "./LogMessages";
import { State } from "../reducers";
import * as actions from "../actions";

export function Main(props: State & typeof actions)
{
    console.log(props);
    return (
    <div>
        {props.packageDoc.logs["nito.asyncex/4.0.1/net45"] ? <LogMessages messages={props.packageDoc.logs["nito.asyncex/4.0.1/net45"]} currentTimestamp={props.time.timestamp} /> : null}
        <button onClick={() => props.DocActions.getDoc({ packageId: "Nito.AsyncEx" })}>Click me!</button>
    </div>);
}