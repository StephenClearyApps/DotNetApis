import { IParameter, MethodParameterModifiers } from "../../structure";
import { ReactFragment, FormatContext, array, keyword, literal, typeReference, attribute } from ".";

export function parameter(context: FormatContext, value: IParameter, attributeDivider?: ReactFragment): ReactFragment {
    attributeDivider = attributeDivider || ' ';
    return [
        array(value.b).map(x => [attribute(context, x), attributeDivider]),
        context.includeParameterModifiers ?
                (value.m === MethodParameterModifiers.OUT ? [keyword('out'), ' '] :
                value.m === MethodParameterModifiers.REF ? [keyword('ref'), ' '] :
                value.m === MethodParameterModifiers.PARAMS ? [keyword('params'), ' '] : null)
            : null,
        [typeReference(context, value.t), ' '],
        value.n,
        value.v === undefined ? null : [' = ', literal(context, value.v)]
    ];
}
