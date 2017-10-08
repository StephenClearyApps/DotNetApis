import * as React from "react";

import { PackageDoc } from "../util";
import { ReactFragment, FormatContext, Styles, join, array } from "./util";
import { IEntity, isClass, isInterface, isStruct, isEnum, isDelegate, isMethod, isProperty, isEvent, isField, ITypeEntity, IEnumEntity, IEnumField, IDelegateEntity, IMethodEntity, MethodStyles, IPropertyEntity, IEventEntity, IFieldEntity, EntityModifiers } from "../structure";
import { attribute } from "./attribute";
import { keyword } from "./keyword";
import { nameWithGenericParameters } from "./nameWithGenericParameters";
import { typeReference } from "./typeReference";
import { accessibility } from "./accessibility";
import { modifiers } from "./modifiers";
import { genericParameterConstraint } from "./genericParameterConstraint";
import { parameter } from "./parameter";
import { literal } from "./literal";

export function declaration(pkg: PackageDoc, entity: IEntity): ReactFragment {
    const context = new FormatContext(pkg, Styles.DECLARATION);
    if (isClass(entity))
        return typeDeclaration(context, entity, 'class');
    else if (isInterface(entity))
        return typeDeclaration(context, entity, 'interface');
    else if (isStruct(entity))
        return typeDeclaration(context, entity, 'struct');
    else if (isEnum(entity))
        return enumDeclaration(context, entity);
    else if (isDelegate(entity))
        return delegateDeclaration(context, entity);
    else if (isMethod(entity))
        return methodDeclaration(context, entity);
    else if (isProperty(entity))
        return propertyDeclaration(context, entity);
    else if (isEvent(entity))
        return eventDeclaration(context, entity);
    else if (isField(entity))
        return fieldDeclaration(context, entity);
}

function typeDeclaration(context: FormatContext, entity: ITypeEntity, typeKeyword: string): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
        accessibility(entity.a),
        modifiers(entity.m),
        keyword(typeKeyword), ' ', nameWithGenericParameters(context, entity),
        entity.t ? [': ', join(entity.t.map(x => typeReference(context, x)), ', ')] : null,
        entity.g ? entity.g.map(x => genericParameterConstraint(context, x)) : null
    ];
}

function enumDeclaration(context: FormatContext, entity: IEnumEntity): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
        accessibility(entity.a),
        keyword('enum'), ' ', entity.n,
        entity.u ? [': ', entity.u] : null,
        <br/>,
        '{',
        entity.f ? entity.f.map(x => enumField(x, entity)) : null,
        <br/>,
        '}'
    ];
}

function enumField(value: IEnumField, entity: IEnumEntity): ReactFragment {
    return [
        <br/>,
        '    ' + value.n + ' = ',
        entity.h ? value.v.toString(16) : value.v.toString()
    ];
}

function delegateDeclaration(context: FormatContext, entity: IDelegateEntity): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
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

function methodDeclaration(context: FormatContext, entity: IMethodEntity): ReactFragment {
    const result = [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
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

function propertyDeclaration(context: FormatContext, entity: IPropertyEntity): ReactFragment {
    const result = [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
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
        result.push(<br/>, '{');
        if (entity.g) {
            result.push(<br/>, '  ',
                entity.g.b ? entity.g.b.map(x => [attribute(context, x), <br/>, '  ']) : null,
                accessibility(entity.g.a),
                keyword('get'), ';', <br/>);
        }
        if (entity.s) {
            result.push(<br/>, '  ',
                entity.s.b ? entity.s.b.map(x => [attribute(context, x), <br/>, '  ']) : null,
                accessibility(entity.s.a),
                keyword('set'), ';', <br/>);
        }
        result.push('}');
    }

    return result;
}

function eventDeclaration(context: FormatContext, entity: IEventEntity): ReactFragment {
    const result = [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
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

function fieldDeclaration(context: FormatContext, entity: IFieldEntity): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
        accessibility(entity.a),
        modifiers(entity.m),
        typeReference(context, entity.t), ' ',
        entity.n,
        (entity.m & EntityModifiers.CONST) ? [' = ', literal(context, entity.v)] : null
    ];
}
