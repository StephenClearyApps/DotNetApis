import * as React from "react";

import { IPropertyEntity, EntityModifiers } from "../../structure";
import { FormatContext, ReactFragment, attribute, accessibility, modifiers, typeReference, keyword, join, parameter, Styles } from ".";

export function propertyDeclaration(context: FormatContext, entity: IPropertyEntity): ReactFragment {
    switch (context.style) {
        case Styles.DECLARATION: return propertyDeclarationFull(context, entity);
        case Styles.MEMBER: return propertyDeclarationSimple(context, entity);
        case Styles.TITLE: return propertyDeclarationTitle(entity);
    }
}    

function propertyDeclarationFull(context: FormatContext, entity: IPropertyEntity): ReactFragment {
    const result = [
        entity.b ? entity.b.map(x => [attribute(context, x), [<br/>]]) : null,
        accessibility(entity.a),
        modifiers(entity.m),
        typeReference(context, entity.t), ' ',
        entity.d ? [typeReference(context, entity.d), '.'] : null,
        entity.n === 'this' ? keyword(entity.n) : entity.n,
        entity.p ? ['[', join(entity.p.map(x => parameter(context, x)), ', '), ']'] : null
    ];

    if (!entity.g && !entity.s)
        return result;

    const getAttributes = entity.g && entity.g.b;
    const setAttributes = entity.s && entity.s.b;
    if (!getAttributes && !setAttributes) {
        result.push(' { ');
        if (entity.g) {
            result.push(accessibility(entity.g.a), keyword('get'), '; ');
        }
        if (entity.s) {
            result.push(accessibility(entity.s.a), keyword('set'), '; ');
        }
        result.push('}');
    } else {
        result.push([<br/>], '{');
        if (entity.g) {
            result.push([<br/>], '  ',
                entity.g.b ? entity.g.b.map(x => [attribute(context, x), [<br/>], '  ']) : null,
                accessibility(entity.g.a),
                keyword('get'), ';', [<br/>]);
        }
        if (entity.s) {
            result.push([<br/>], '  ',
                entity.s.b ? entity.s.b.map(x => [attribute(context, x), [<br/>], '  ']) : null,
                accessibility(entity.s.a),
                keyword('set'), ';', [<br/>]);
        }
        result.push('}');
    }

    return result;
}

function propertyDeclarationSimple(context: FormatContext, entity: IPropertyEntity): ReactFragment {
    const result = [
        accessibility(entity.a),
        modifiers(entity.m),
        typeReference(context, entity.t), ' ',
        entity.d ? [typeReference(context, entity.d), '.'] : null,
        entity.n === 'this' ? keyword(entity.n) : entity.n,
        entity.p ? ['[', join(entity.p.map(x => parameter(context, x)), ', '), ']'] : null
    ];

    if (!entity.g && !entity.s)
        return result;

    result.push(' { ');
    if (entity.g) {
        result.push(accessibility(entity.g.a), keyword('get'), '; ');
    }
    if (entity.s) {
        result.push(accessibility(entity.s.a), keyword('set'), '; ');
    }
    result.push('}');

    return result;
}

function propertyDeclarationTitle(entity: IPropertyEntity): ReactFragment {
    return [
        [<code>{entity.n}</code>],
        entity.m & EntityModifiers.STATIC ? ' Static' : null,
        ' Property'
    ];
}