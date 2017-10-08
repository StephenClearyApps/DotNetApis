import { IConcreteTypeReference } from "../structure";
import { ReactFragment, FormatContext, locationDnaid, join } from "./util";
import { locationLink } from "./locationLink";
import { typeReference } from "./typeReference";

export function concreteTypeReference(context: FormatContext, entity: IConcreteTypeReference): ReactFragment {
    if (locationDnaid(entity.l) === 'System.Nullable\'1')
        return [typeReference(context, entity.a[0]), locationLink(context, entity.l, '?')];
    if (entity.a)
        return [locationLink(context, entity.l, entity.n), '<', join(entity.a.map(x => typeReference(context, x)), ', '), '>'];
    return locationLink(context, entity.l, entity.n);
}
