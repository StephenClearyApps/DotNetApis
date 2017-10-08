import { IAttribute, IAttributeArgument } from "../structure";
import { ReactFragment, FormatContext, join } from "./util";
import { keyword } from "./keyword";
import { literal } from "./literal";
import { locationLink } from "./locationLink";

function attributeTarget(entity: IAttribute): ReactFragment {
    return entity.t ? [keyword(entity.t), ': '] : null;
}

function attributeConstructorArgument(context: FormatContext, entity: IAttributeArgument): ReactFragment {
    if (entity.n)
        return [entity.n, ' = ', literal(context, entity.v)]
    else
        return literal(context, entity.v);
}

export function attribute(context: FormatContext, entity: IAttribute): ReactFragment {
    return [
        '[',
        attributeTarget(entity),
        locationLink(context, entity.l, entity.n),
        entity.a ?
            ['(', join(entity.a.map(x => attributeConstructorArgument(context, x)), ', '), ')'] :
            null,
        ']'
    ];
}
