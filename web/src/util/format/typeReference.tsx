import * as React from "react";

import { ITypeReference, isDynamic, isKeyword, isGenericParameter, isRequiredModifier, isPointer, isArray, IArrayDimension, isGenericInstance, isSimpleOrOpenGeneric } from "../structure";
import { ReactFragment, FormatContext, locationDnaid, join } from "./util";
import { keyword } from "./keyword";
import { locationLink } from "./locationLink";
import { concreteTypeReference } from "./concreteTypeReference";

function arrayDimension(dim: IArrayDimension): string {
    return dim.u ? dim.u.toString() : '';
}

export function typeReference(context: FormatContext, entity: ITypeReference): ReactFragment {
    if (isDynamic(entity))
        return keyword('dynamic');
    else if (isKeyword(entity))
        return locationLink(context, entity.l, keyword(entity.n));
    else if (isGenericParameter(entity))
        return entity.n; // TODO: syntax highlighting for this?
    else if (isRequiredModifier(entity))
        return locationDnaid(entity.l) === 'System.Runtime.CompilerServices.IsVolatile' ?
            [locationLink(context, entity.l, keyword('volatile')), ' ', typeReference(context, entity.t)] :
            typeReference(context, entity.t);
    else if (isPointer(entity))
        return [typeReference(context, entity.t), '*'];
    else if (isArray(entity))
        return [typeReference(context, entity.t), '[', entity.d.map(x => arrayDimension(x)).join(','), ']'];
    else if (isGenericInstance(entity))
        return join(entity.t.map(x => concreteTypeReference(context, x)), '.');
    else if (isSimpleOrOpenGeneric(entity))
        return [
            entity.t ? [typeReference(context, entity.t), '.'] : null,
            locationLink(context, entity.l, entity.n),
            entity.a ? ['<', new Array(entity.a).join(','), '>'] : null
        ];
}
