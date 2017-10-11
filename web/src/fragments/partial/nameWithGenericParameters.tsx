import { ITypeEntity, IMethodEntity } from "../../structure";
import { ReactFragment, FormatContext, join, genericParameter } from ".";

export function nameWithGenericParameters(context: FormatContext, entity: ITypeEntity | IMethodEntity): ReactFragment {
    if (!entity.g)
        return entity.n;
    return [
        entity.n,
        '<', join(entity.g.map(x => genericParameter(context, x)), ', '), '>'
    ];
}
