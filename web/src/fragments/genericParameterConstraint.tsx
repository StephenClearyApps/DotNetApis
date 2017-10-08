import * as React from "react";

import { IGenericParameter, IGenericConstraint, isClassGenericConstrint, isNewGenericConstrint, isStructGenericConstrint, isTypeGenericConstrint } from "../structure";
import { ReactFragment, FormatContext, join } from "./util";
import { keyword } from "./keyword";
import { typeReference } from "./typeReference";

export function genericParameterConstraint(context: FormatContext, value: IGenericParameter): ReactFragment {
    if (!value.c)
        return null;
    return [
        <br/>,
        '    ',
        keyword('where'),
        ' ' + value.n + ': ',
        join(value.c.map(x => genericConstraint(context, x)), ', ')
    ];
}

function genericConstraint(context: FormatContext, value: IGenericConstraint): ReactFragment {
    if (isClassGenericConstrint(value))
        return keyword('class');
    else if (isNewGenericConstrint(value))
        return [keyword('new'), '()'];
    else if (isStructGenericConstrint(value))
        return keyword('struct');
    else if (isTypeGenericConstrint(value))
        return typeReference(context, value.t);
}
