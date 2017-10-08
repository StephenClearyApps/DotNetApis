import { IGenericParameter, GenericParameterModifiers } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { keyword } from "./keyword";

export function genericParameter(context: FormatContext, value: IGenericParameter): ReactFragment {
    if (!context.includeParameterModifiers)
        return value.n;
    switch (value.m) {
        case GenericParameterModifiers.IN: return [keyword('in'), ' ', value.n];
        case GenericParameterModifiers.OUT: return [keyword('out'), ' ', value.n];
        default: return value.n;
    }
}
