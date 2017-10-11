import * as React from "react";

import { ITypeEntity, EntityModifiers } from "../../structure";
import { FormatContext, ReactFragment, attribute, accessibility, modifiers, keyword, nameWithGenericParameters, join, typeReference, genericParameterConstraint, Styles } from ".";

export function typeDeclaration(context: FormatContext, entity: ITypeEntity, typeKeyword: string, ns?: string): ReactFragment {
    switch (context.style) {
        case Styles.DECLARATION: return typeDeclarationFull(context, entity, typeKeyword);
        case Styles.MEMBER: return typeDeclarationSimple(context, entity, typeKeyword, ns);
        case Styles.TITLE: return typeDeclarationTitle(context, entity, typeKeyword);
    }
}    

function typeDeclarationFull(context: FormatContext, entity: ITypeEntity, typeKeyword: string): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), [<br/>]]) : null,
        accessibility(entity.a),
        modifiers(entity.m),
        keyword(typeKeyword), ' ', nameWithGenericParameters(context, entity),
        entity.t ? [': ', join(entity.t.map(x => typeReference(context, x)), ', ')] : null,
        entity.g ? entity.g.map(x => genericParameterConstraint(context, x)) : null
    ];
}

function typeDeclarationSimple(context: FormatContext, entity: ITypeEntity, typeKeyword: string, ns: string): ReactFragment {
    return [
        (entity.m & EntityModifiers.STATIC) ? [keyword("static"), " "] : null,
        keyword(typeKeyword), ' ', nameWithGenericParameters(context, entity),
        ns ? " (" + ns + ")" : null
    ];
}

function typeDeclarationTitle(context: FormatContext, entity: ITypeEntity, typeKeyword: string): ReactFragment {
    return [
        [<code>{React.Children.toArray(nameWithGenericParameters(context, entity))}</code>],
        entity.m & EntityModifiers.STATIC ? ' Static' : null,
        ' ' + typeKeyword
    ];
}
