import { EntityModifiers } from "../structure";
import { ReactFragment } from "./util";
import { keyword } from "./keyword";

export function modifiers(value: EntityModifiers): ReactFragment {
    const result = [];
    if (value & EntityModifiers.CONST)
        result.push([keyword('const'), ' ']);
    if (value & EntityModifiers.STATIC)
        result.push([keyword('static'), ' ']);
    if (value & EntityModifiers.ABSTRACT)
        result.push([keyword('abstract'), ' ']);
    if (value & EntityModifiers.SEALED)
        result.push([keyword('sealed'), ' ']);
    if (value & EntityModifiers.VIRTUAL)
        result.push([keyword('virtual'), ' ']);
    if (value & EntityModifiers.OVERRIDE)
        result.push([keyword('override'), ' ']);
    return result;
}
