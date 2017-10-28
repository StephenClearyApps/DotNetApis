import * as React from "react";

import { FrontPagePackages } from "./FrontPagePackages";

import { State } from "../reducers";
import { Actions } from "../actions";

export function Home(props: State & Actions)
{
    return (
    <div>
        <FrontPagePackages/>
    </div>);
}
