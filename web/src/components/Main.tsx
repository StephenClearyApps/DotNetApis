import * as React from "react";

import { State } from "../reducers";

export const Main = (props: State) => <h1>Hello {JSON.stringify(props)}!</h1>;