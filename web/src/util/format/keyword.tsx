import * as React from "react";

import { ReactFragment } from "./util";

export function keyword(name: string): ReactFragment {
    return <span className='k'>{name}</span>;
}
