import { ITypeEntity } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { location } from "./location";
import { nameWithGenericParameters } from "./nameWithGenericParameters";

export function fullConcreteTypeReference(context: FormatContext, entity: ITypeEntity): ReactFragment {
    const parent = context.pkg.findEntityParent(entity.i);
    if (!parent)
        return [entity.s, '.', location(context, entity.i, nameWithGenericParameters(context, entity))];
    const parentFragment = fullConcreteTypeReference(context, parent);
    return [parentFragment, '.', location(context, entity.i, nameWithGenericParameters(context, entity))];
}
