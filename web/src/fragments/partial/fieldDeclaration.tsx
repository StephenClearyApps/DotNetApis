import * as React from "react";

import { IFieldEntity, EntityModifiers } from "../../structure";
import { FormatContext, ReactFragment, attribute, accessibility, modifiers, typeReference, literal, keyword, Styles } from ".";

export function fieldDeclaration(context: FormatContext, entity: IFieldEntity): ReactFragment {
    switch (context.style) {
        case Styles.DECLARATION: return fieldDeclarationFull(context, entity);
        case Styles.MEMBER: return fieldDeclarationSimple(context, entity);
        case Styles.TITLE: return fieldDeclarationTitle(entity);
    }
}    

function fieldDeclarationFull(context: FormatContext, entity: IFieldEntity): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), [<br/>]]) : null,
        accessibility(entity.a),
        modifiers(entity.m),
        typeReference(context, entity.t), ' ',
        entity.n,
        (entity.m & EntityModifiers.CONST) ? [' = ', literal(context, entity.v)] : null
    ];
}

function fieldDeclarationSimple(context: FormatContext, entity: IFieldEntity): ReactFragment {
    return [
        accessibility(entity.a),
        (entity.m & EntityModifiers.STATIC) ? [keyword("static"), " "] : null,
        typeReference(context, entity.t), ' ',
        entity.n
    ];
}

function fieldDeclarationTitle(entity: IFieldEntity): ReactFragment {
    return [
        [<code>{entity.n}</code>],
        entity.m & EntityModifiers.STATIC ? ' Static' : null,
        ' Field'
    ];
}