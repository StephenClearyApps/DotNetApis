import * as React from "react";

import { IDelegateEntity } from "../../structure";
import { FormatContext, ReactFragment, attribute, accessibility, keyword, typeReference, nameWithGenericParameters, join, array, parameter, genericParameterConstraint, Styles } from ".";

export function delegateDeclaration(context: FormatContext, entity: IDelegateEntity, ns?: string): ReactFragment {
    switch (context.style) {
        case Styles.DECLARATION: return delegateDeclarationFull(context, entity);
        case Styles.MEMBER: return delegateDeclarationSimple(context, entity, ns);
        case Styles.TITLE: return delegateDeclarationTitle(context, entity);
    }
}

function delegateDeclarationFull(context: FormatContext, entity: IDelegateEntity): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), [<br/>]]) : null,
        accessibility(entity.a),
        keyword('delegate'), ' ',
        typeReference(context, entity.r), ' ',
        nameWithGenericParameters(context, entity),
        '(',
        join(array(entity.p).map(x => parameter(context, x)), ', '),
        ')',
        entity.g ? entity.g.map(x => genericParameterConstraint(context, x)) : null
    ];
}

function delegateDeclarationSimple(context: FormatContext, entity: IDelegateEntity, ns: string | undefined): ReactFragment {
    return [
        keyword('delegate'), ' ',
        typeReference(context, entity.r), ' ',
        nameWithGenericParameters(context, entity),
        '(',
        join(array(entity.p).map(x => parameter(context, x)), ', '),
        ')',
        ns ? " (" + ns + ")" : null
    ];
}

function delegateDeclarationTitle(context: FormatContext, entity: IDelegateEntity): ReactFragment {
    return [
        [<code>{React.Children.toArray(nameWithGenericParameters(context, entity))}</code>],
        ' Delegate'
    ];
}
