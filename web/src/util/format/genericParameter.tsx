import { IGenericParameter, GenericParameterModifiers } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { keyword } from "./keyword";

export function genericParameter(context: FormatContext, parameter: IGenericParameter): ReactFragment {
    if (!context.includeParameterModifiers)
        return parameter.n;
    switch (parameter.m) {
        case GenericParameterModifiers.IN: return [keyword('in'), ' ', parameter.n];
        case GenericParameterModifiers.OUT: return [keyword('out'), ' ', parameter.n];
        default: return parameter.n;
    }
}
