import { ITypeEntity } from "../../structure";
import { ReactFragment, FormatContext, location, nameWithGenericParameters } from ".";

export function fullConcreteTypeReference(context: FormatContext, entity: ITypeEntity): ReactFragment {
    const parent = context.pkg.findEntityParent(entity.i);
    if (!parent)
        return [entity.s, '.', location(context, entity.i, nameWithGenericParameters(context, entity))];
    const parentFragment = fullConcreteTypeReference(context, parent);
    return [parentFragment, '.', location(context, entity.i, nameWithGenericParameters(context, entity))];
}
