import * as React from "react";

import { IEventEntity, EntityModifiers } from "../../structure";
import { FormatContext, ReactFragment, attribute, accessibility, modifiers, keyword, typeReference, Styles } from ".";

export function eventDeclaration(context: FormatContext, entity: IEventEntity): ReactFragment {
    switch (context.style) {
        case Styles.DECLARATION: return eventDeclarationFull(context, entity);
        case Styles.MEMBER: return eventDeclarationSimple(context, entity);
        case Styles.TITLE: return eventDeclarationTitle(entity);
    }
}

function eventDeclarationFull(context: FormatContext, entity: IEventEntity): ReactFragment {
    const result = [
        entity.b ? entity.b.map(x => [attribute(context, x), [<br/>]]) : null,
        accessibility(entity.a),
        modifiers(entity.m),
        keyword('event'), ' ',
        typeReference(context, entity.t), ' ',
        entity.d ? [typeReference(context, entity.d), '.'] : null,
        entity.n
    ];

    if (!entity.p && !entity.r)
        return result;

    result.push(' { ',
        entity.p ? entity.p.map(x => [attribute(context, x), ' ']) : null,
        keyword('add'), '; ',
        entity.r ? entity.r.map(x => [attribute(context, x), ' ']) : null,
        keyword('remove'), '; }'
    );

    return result;
}

function eventDeclarationSimple(context: FormatContext, entity: IEventEntity): ReactFragment {
    return [
        accessibility(entity.a),
        modifiers(entity.m),
        keyword('event'), ' ',
        typeReference(context, entity.t), ' ',
        entity.d ? [typeReference(context, entity.d), '.'] : null,
        entity.n
    ];
}

function eventDeclarationTitle(entity: IEventEntity): ReactFragment {
    return [
        [<code>{entity.n}</code>],
        entity.m & EntityModifiers.STATIC ? ' Static' : null,
        ' Event'
    ];
}