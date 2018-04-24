import * as React from "react";

import { IMethodEntity, MethodStyles, EntityModifiers } from "../../structure";
import { FormatContext, ReactFragment, attribute, accessibility, modifiers, keyword, typeReference, join, array, parameter, nameWithGenericParameters, genericParameterConstraint, Styles } from ".";

export function methodDeclaration(context: FormatContext, entity: IMethodEntity): ReactFragment {
    switch (context.style) {
        case Styles.DECLARATION: return methodDeclarationFull(context, entity);
        case Styles.MEMBER: return methodDeclarationSimple(context, entity);
        case Styles.TITLE: return methodDeclarationTitle(context, entity);
    }
}    

function methodDeclarationFull(context: FormatContext, entity: IMethodEntity): ReactFragment {
    const result = [
        entity.b ? entity.b.map(x => [attribute(context, x), [<br/>]]) : null,
        accessibility(entity.a),
        modifiers(entity.m)
    ];

    if (entity.s === MethodStyles.IMPLICIT || entity.s === MethodStyles.EXPLICIT) {
        result.push(keyword(entity.s === MethodStyles.IMPLICIT ? 'implicit' : 'explicit'), ' ',
            keyword('operator'), ' ',
            typeReference(context, entity.r),
            '(', join(array(entity.p).map(x => parameter(context, x)), ', '), ')'
        );
        return result;
    }

    if (entity.r)
        result.push(typeReference(context, entity.r), ' ');
    if (entity.s === MethodStyles.OPERATOR)
        result.push(keyword('operator'), ' ');
    if (entity.d)
        result.push(typeReference(context, entity.d), '.');
    result.push(nameWithGenericParameters(context, entity));
    result.push('(');
    if (entity.s === MethodStyles.EXTENSION)
        result.push(keyword('this'), ' ');
    result.push(join(array(entity.p).map(x => parameter(context, x)), ', '), ')');
    if (entity.g)
        result.push(entity.g.map(x => genericParameterConstraint(context, x)));
    return result;
}

function methodDeclarationSimple(context: FormatContext, entity: IMethodEntity): ReactFragment {
    const result = [
        accessibility(entity.a),
        modifiers(entity.m)
    ];

    if (entity.s === MethodStyles.IMPLICIT || entity.s === MethodStyles.EXPLICIT) {
        result.push(keyword(entity.s === MethodStyles.IMPLICIT ? 'implicit' : 'explicit'), ' ',
            keyword('operator'), ' ',
            typeReference(context, entity.r),
            '(', join(array(entity.p).map(x => parameter(context, x)), ', '), ')'
        );
        return result;
    }

    if (entity.r)
        result.push(typeReference(context, entity.r), ' ');
    if (entity.s === MethodStyles.OPERATOR)
        result.push(keyword('operator'), ' ');
    if (entity.d)
        result.push(typeReference(context, entity.d), '.');
    result.push(nameWithGenericParameters(context, entity));
    result.push('(');
    if (entity.s === MethodStyles.EXTENSION)
        result.push(keyword('this'), ' ');
    result.push(join(array(entity.p).map(x => parameter(context, x)), ', '), ')');
    return result;
}

function methodDeclarationTitle(context: FormatContext, entity: IMethodEntity): ReactFragment {
    if (entity.s === MethodStyles.IMPLICIT || entity.s === MethodStyles.EXPLICIT) {
        return [
            [<code>{React.Children.toArray(typeReference(context, entity.r))}({React.Children.toArray(join(array(entity.p).map(x => parameter(context, x)), ', '))})</code>],
            ' Operator'
        ];
    }

    return [
        [<code>{React.Children.toArray(nameWithGenericParameters(context, entity))}({React.Children.toArray(join(array(entity.p).map(x => parameter(context, x)), ', '))})</code>],
        entity.s === MethodStyles.OPERATOR ? ' Operator' :
            [entity.m && (entity.m & EntityModifiers.STATIC) ? ' Static' : null, ' Method']
    ];
}
