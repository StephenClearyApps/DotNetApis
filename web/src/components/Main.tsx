import * as React from "react";

import { State } from "../reducers";
import * as actions from "../actions";

export function Main(props: State & typeof actions)
{
    console.log(props);
    return (
    <div>
        <h1>Hello {JSON.stringify(props)}!</h1>
        <button onClick={() => props.DocActions.getDoc({ packageId: "Nito.AsyncEx" })}>Click me!</button>
    </div>);
}