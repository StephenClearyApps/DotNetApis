import * as React from "react";

import { PackageDoc } from "../util";
import { IEntity, isClass, ITypeEntity, EntityModifiers, isInterface, isStruct, isEnum, IEnumEntity,
    isDelegate, isMethod, isProperty, isEvent, isField, IDelegateEntity, IMethodEntity, MethodStyles, IPropertyEntity, IEventEntity, IFieldEntity
} from "../structure";
import { nameWithGenericParameters } from "./nameWithGenericParameters";
import { ReactFragment, FormatContext, Styles, join, array } from "./util";
import { typeReference } from "./typeReference";
import { parameter } from "./parameter";

export function formatTitle(pkg: PackageDoc, entity: IEntity): ReactFragment {
    const context = new FormatContext(pkg, Styles.TITLE);
    if (isClass(entity))
        return titleTypeDeclaration(context, entity, "Class");
    else if (isInterface(entity))
        return titleTypeDeclaration(context, entity, 'Interface');
    else if (isStruct(entity))
        return titleTypeDeclaration(context, entity, 'Struct');
    else if (isEnum(entity))
        return titleEnumDeclaration(entity);
    else if (isDelegate(entity))
        return titleDelegateDeclaration(context, entity);
    else if (isMethod(entity))
        return titleMethodDeclaration(context, entity);
    else if (isProperty(entity))
        return titleSimpleMemberDeclaration(entity, "Property");
    else if (isEvent(entity))
        return titleSimpleMemberDeclaration(entity, "Event");
    else if (isField(entity))
        return titleSimpleMemberDeclaration(entity, "Field");
}

function titleTypeDeclaration(context: FormatContext, entity: ITypeEntity, typeKeyword: string): ReactFragment {
    return [
        <code>{nameWithGenericParameters(context, entity)}</code>,
        entity.m & EntityModifiers.STATIC ? ' Static' : null,
        ' ' + typeKeyword
    ];
}

function titleEnumDeclaration(entity: IEnumEntity): ReactFragment {
    return [
        <code>{entity.n}</code>,
        ' Enum'
    ];
}

function titleDelegateDeclaration(context: FormatContext, entity: IDelegateEntity): ReactFragment {
    return [
        <code>{nameWithGenericParameters(context, entity)}</code>,
        ' Delegate'
    ];
}

function titleMethodDeclaration(context: FormatContext, entity: IMethodEntity): ReactFragment {
    if (entity.s === MethodStyles.IMPLICIT || entity.s === MethodStyles.EXPLICIT) {
        return [
            <code>{typeReference(context, entity.r)}({join(array(entity.p).map(x => parameter(context, x)), ', ')})</code>,
            ' Operator'
        ];
    }

    return [
        <code>{nameWithGenericParameters(context, entity)}({join(array(entity.p).map(x => parameter(context, x)), ', ')})</code>,
        entity.s === MethodStyles.OPERATOR ? ' Operator' :
            [entity.m & EntityModifiers.STATIC ? ' Static' : null, ' Method']
    ];
}

function titleSimpleMemberDeclaration(entity: IPropertyEntity | IEventEntity | IFieldEntity, typeKeyword: string): ReactFragment {
    return [
        <code>{entity.n}</code>,
        entity.m & EntityModifiers.STATIC ? ' Static' : null,
        ' ' + typeKeyword
    ];
}
