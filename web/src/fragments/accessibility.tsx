import { EntityAccessibility } from "../structure";
import { ReactFragment } from "./util";
import { keyword } from "./keyword";

export function accessibility(value: EntityAccessibility): ReactFragment {
    switch (value) {
        case EntityAccessibility.PROTECTED: return [keyword('protected'), ' '];
        case EntityAccessibility.HIDDEN: return null;
        default: return [keyword('public'), ' '];
    }
}
