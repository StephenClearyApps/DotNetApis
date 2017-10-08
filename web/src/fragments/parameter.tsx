import { IParameter, MethodParameterModifiers } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { keyword } from "./keyword";
import { literal } from "./literal";
import { typeReference } from "./typeReference";
import { attribute } from "./attribute";

export function parameter(context: FormatContext, entity: IParameter, attributeDivider?: ReactFragment): ReactFragment {
    attributeDivider = attributeDivider || ' ';
    return [
        entity.b.map(x => [attribute(context, x), attributeDivider]),
        context.includeParameterModifiers ?
                (entity.m === MethodParameterModifiers.OUT ? [keyword('out'), ' '] :
                entity.m === MethodParameterModifiers.REF ? [keyword('ref'), ' '] :
                entity.m === MethodParameterModifiers.PARAMS ? [keyword('params'), ' '] : null)
            : null,
        [typeReference(context, entity.t), ' '],
        entity.n,
        entity.v === undefined ? null : [' = ', literal(context, entity.v)]
    ];
}
