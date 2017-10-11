import * as React from "react";

import { IEnumEntity, IEnumField } from "../../structure";
import { FormatContext, ReactFragment, attribute, accessibility, keyword, Styles } from ".";

export function enumDeclaration(context: FormatContext, entity: IEnumEntity, ns?: string): ReactFragment {
    switch (context.style) {
        case Styles.DECLARATION: return enumDeclarationFull(context, entity);
        case Styles.MEMBER: return enumDeclarationSimple(context, entity, ns);
        case Styles.TITLE: return enumDeclarationTitle(entity);
    }
}    

function enumDeclarationFull(context: FormatContext, entity: IEnumEntity): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), [<br/>]]) : null,
        accessibility(entity.a),
        keyword('enum'), ' ', entity.n,
        entity.u ? [': ', entity.u] : null,
        [<br/>],
        '{',
        entity.f ? entity.f.map(x => enumField(x, entity)) : null,
        [<br/>],
        '}'
    ];
}

function enumField(value: IEnumField, entity: IEnumEntity): ReactFragment {
    return [
        [<br/>],
        '    ' + value.n + ' = ',
        entity.h ? value.v.toString(16) : value.v.toString()
    ];
}

function enumDeclarationSimple(context: FormatContext, entity: IEnumEntity, ns: string): ReactFragment {
    return [
        keyword('enum'), ' ', entity.n,
        ns ? " (" + ns + ")" : null
    ];
}

function enumDeclarationTitle(entity: IEnumEntity): ReactFragment {
    return [
        [<code>{entity.n}</code>],
        ' Enum'
    ];
}
