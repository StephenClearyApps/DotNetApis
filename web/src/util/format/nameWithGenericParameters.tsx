import * as React from "react";

import { ITypeEntity, IMethodEntity } from "../structure";
import { ReactFragment, FormatContext, join } from "./util";
import { genericParameter } from "./genericParameter";

export function nameWithGenericParameters(context: FormatContext, entity: ITypeEntity | IMethodEntity): ReactFragment {
    if (!entity.g)
        return entity.n;
    return [
        entity.n,
        '<', join(entity.g.map(x => genericParameter(context, x)), ', '), '>'
    ];
}
