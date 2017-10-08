import * as React from "react";

import { ILiteral, isNullLiteral, isArrayLiteral, isTypeofLiteral, isPrimitiveLiteral, isEnumLiteral, IPrimitiveLiteral, IEnumLiteral } from "../structure";
import { ReactFragment, FormatContext, join } from "./util";
import { keyword } from "./keyword";
import { typeReference } from "./typeReference";

function valueString(entity: IPrimitiveLiteral | IEnumLiteral): ReactFragment {
    if (typeof(entity.v) === 'string')
        return <span className='s'>{'"' + entity.v + '"'}</span>;
    return entity.h ? entity.v.toString(16) : entity.v.toString();
}

export function literal(context: FormatContext, entity: ILiteral): ReactFragment {
    if (isNullLiteral(entity))
        return keyword('null');
    else if (isArrayLiteral(entity))
        return [
            keyword('new'),
            ' ',
            typeReference(context, entity.t),
            '[] { ',
            join(entity.v.map(x => literal(context, x)), ', '),
            ' }'
        ];
    else if (isTypeofLiteral(entity))
        return [keyword('typeof'), '(', typeReference(context, entity.t), ')'];
    else if (isPrimitiveLiteral(entity))
        return valueString(entity);
    else if (isEnumLiteral(entity))
        return entity.n ?
            join(entity.n.map(x => [typeReference(context, entity.t), '.', x]), ' | ') :
            valueString(entity);
}
