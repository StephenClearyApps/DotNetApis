import { PackageDoc } from "../util";
import { ReactFragment, FormatContext, Styles, join, array } from "./util";
import { IEntity, isClass, isInterface, isStruct, isEnum, isDelegate, isMethod, isProperty, isEvent, isField, ITypeEntity, IEnumEntity, IDelegateEntity, IMethodEntity, IPropertyEntity, IEventEntity, IFieldEntity, EntityModifiers, MethodStyles } from "../structure";
import { keyword } from "./keyword";
import { nameWithGenericParameters } from "./nameWithGenericParameters";
import { typeReference } from "./typeReference";
import { parameter } from "./parameter";
import { accessibility } from "./accessibility";
import { modifiers } from "./modifiers";

export function simpleDeclaration(pkg: PackageDoc, entity: IEntity, ns?: string): ReactFragment {
    const context = new FormatContext(pkg, Styles.MEMBER);
    if (isClass(entity))
        return typeDeclaration(context, entity, 'class', ns);
    else if (isInterface(entity))
        return typeDeclaration(context, entity, 'interface', ns);
    else if (isStruct(entity))
        return typeDeclaration(context, entity, 'struct', ns);
    else if (isEnum(entity))
        return enumDeclaration(context, entity, ns);
    else if (isDelegate(entity))
        return delegateDeclaration(context, entity, ns);
    else if (isMethod(entity))
        return methodDeclaration(context, entity);
    else if (isProperty(entity))
        return propertyDeclaration(context, entity);
    else if (isEvent(entity))
        return eventDeclaration(context, entity);
    else if (isField(entity))
        return fieldDeclaration(context, entity);
}

function typeDeclaration(context: FormatContext, entity: ITypeEntity, typeKeyword: string, ns: string): ReactFragment {
    return [
        (entity.m & EntityModifiers.STATIC) ? [keyword("static"), " "] : null,
        keyword(typeKeyword), ' ', nameWithGenericParameters(context, entity),
        ns ? " (" + ns + ")" : null
    ];
}

function enumDeclaration(context: FormatContext, entity: IEnumEntity, ns: string): ReactFragment {
    return [
        keyword('enum'), ' ', entity.n,
        ns ? " (" + ns + ")" : null
    ];
}

function delegateDeclaration(context: FormatContext, entity: IDelegateEntity, ns: string): ReactFragment {
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

function methodDeclaration(context: FormatContext, entity: IMethodEntity): ReactFragment {
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

function propertyDeclaration(context: FormatContext, entity: IPropertyEntity): ReactFragment {
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

function eventDeclaration(context: FormatContext, entity: IEventEntity): ReactFragment {
    return [
        accessibility(entity.a),
        modifiers(entity.m),
        keyword('event'), ' ',
        typeReference(context, entity.t), ' ',
        entity.d ? [typeReference(context, entity.d), '.'] : null,
        entity.n
    ];
}

function fieldDeclaration(context: FormatContext, entity: IFieldEntity): ReactFragment {
    return [
        accessibility(entity.a),
        (entity.m & EntityModifiers.STATIC) ? [keyword("static"), " "] : null,
        typeReference(context, entity.t), ' ',
        entity.n
    ];
}
