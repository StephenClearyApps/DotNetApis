import * as React from "react";

import { ReactFragment } from "./util";

export function keyword(value: string): ReactFragment {
    return [<span className='k'>{value}</span>];
}
