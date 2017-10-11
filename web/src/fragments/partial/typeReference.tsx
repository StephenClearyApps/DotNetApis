import { ITypeReference, isDynamic, isKeyword, isGenericParameter, isRequiredModifier, isPointer, isArray, IArrayDimension, isGenericInstance, isSimpleOrOpenGeneric } from "../../structure";
import { ReactFragment, FormatContext, locationDnaid, join, array, keyword, location, concreteTypeReference } from ".";

function arrayDimension(dim: IArrayDimension): string {
    return dim.u ? dim.u.toString() : '';
}

export function typeReference(context: FormatContext, value: ITypeReference): ReactFragment {
    if (isDynamic(value))
        return keyword('dynamic');
    else if (isKeyword(value))
        return location(context, value.l, keyword(value.n));
    else if (isGenericParameter(value))
        return value.n; // TODO: syntax highlighting for this?
    else if (isRequiredModifier(value))
        return locationDnaid(value.l) === 'System.Runtime.CompilerServices.IsVolatile' ?
            [location(context, value.l, keyword('volatile')), ' ', typeReference(context, value.t)] :
            typeReference(context, value.t);
    else if (isPointer(value))
        return [typeReference(context, value.t), '*'];
    else if (isArray(value))
        return [typeReference(context, value.t), '[', array(value.d).map(x => arrayDimension(x)).join(','), ']'];
    else if (isGenericInstance(value))
        return join(value.t.map(x => concreteTypeReference(context, x)), '.');
    else if (isSimpleOrOpenGeneric(value))
        return [
            value.t ? [typeReference(context, value.t), '.'] : null,
            location(context, value.l, value.n),
            value.a ? ['<', new Array(value.a).join(','), '>'] : null
        ];
}
