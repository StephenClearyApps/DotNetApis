import * as React from "react";

import { ILiteral, isNullLiteral, isArrayLiteral, isTypeofLiteral, isPrimitiveLiteral, isEnumLiteral, IPrimitiveLiteral, IEnumLiteral } from "../structure";
import { ReactFragment, FormatContext, join, array } from "./util";
import { keyword } from "./keyword";
import { typeReference } from "./typeReference";

function valueString(value: IPrimitiveLiteral | IEnumLiteral): ReactFragment {
    if (typeof(value.v) === 'string')
        return [<span className='s'>{'"' + value.v + '"'}</span>];
    return value.h ? value.v.toString(16) : value.v.toString();
}

export function literal(context: FormatContext, value: ILiteral): ReactFragment {
    if (isNullLiteral(value))
        return keyword('null');
    else if (isArrayLiteral(value))
        return [
            keyword('new'),
            ' ',
            typeReference(context, value.t),
            '[] { ',
            join(array(value.v).map(x => literal(context, x)), ', '),
            ' }'
        ];
    else if (isTypeofLiteral(value))
        return [keyword('typeof'), '(', typeReference(context, value.t), ')'];
    else if (isPrimitiveLiteral(value))
        return valueString(value);
    else if (isEnumLiteral(value))
        return value.n ?
            join(value.n.map(x => [typeReference(context, value.t), '.', x]), ' | ') :
            valueString(value);
}
