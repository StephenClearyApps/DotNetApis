import { IConcreteTypeReference } from "../structure";
import { ReactFragment, FormatContext, locationDnaid, join } from "./util";
import { locationLink } from "./locationLink";
import { typeReference } from "./typeReference";

export function concreteTypeReference(context: FormatContext, value: IConcreteTypeReference): ReactFragment {
    if (locationDnaid(value.l) === 'System.Nullable\'1')
        return [typeReference(context, value.a[0]), locationLink(context, value.l, '?')];
    if (value.a)
        return [locationLink(context, value.l, value.n), '<', join(value.a.map(x => typeReference(context, x)), ', '), '>'];
    return locationLink(context, value.l, value.n);
}
