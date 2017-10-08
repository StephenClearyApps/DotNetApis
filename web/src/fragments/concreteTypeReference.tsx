import { IConcreteTypeReference } from "../structure";
import { ReactFragment, FormatContext, locationDnaid, join } from "./util";
import { location } from "./location";
import { typeReference } from "./typeReference";

export function concreteTypeReference(context: FormatContext, value: IConcreteTypeReference): ReactFragment {
    if (locationDnaid(value.l) === 'System.Nullable\'1')
        return [typeReference(context, value.a[0]), location(context, value.l, '?')];
    if (value.a)
        return [location(context, value.l, value.n), '<', join(value.a.map(x => typeReference(context, x)), ', '), '>'];
    return location(context, value.l, value.n);
}
