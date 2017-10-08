import * as React from "react";

import { PackageDoc } from "../util";
import { ReactFragment, FormatContext, Styles, join } from "./util";
import { IEntity, isClass, isInterface, isStruct, isEnum, isDelegate, isMethod, isProperty, isEvent, isField, ITypeEntity } from "../structure";
import { attribute } from "./attribute";
import { keyword } from "./keyword";
import { nameWithGenericParameters } from "./nameWithGenericParameters";
import { typeReference } from "./typeReference";
import { accessibility } from "./accessibility";
import { modifiers } from "./modifiers";

export function declaration(pkg: PackageDoc, entity: IEntity): ReactFragment {
    const context = new FormatContext(pkg, Styles.DECLARATION);
    if (isClass(entity))
        return typeDeclaration(context, entity, 'class');
    else if (isInterface(entity))
        return typeDeclaration(entity, 'interface');
    else if (isStruct(entity))
        return typeDeclaration(entity, 'struct');
    else if (isEnum(entity))
        return enumDeclaration(entity);
    else if (isDelegate(entity))
        return delegateDeclaration(entity);
    else if (isMethod(entity))
        return methodDeclaration(entity);
    else if (isProperty(entity))
        return propertyDeclaration(entity);
    else if (isEvent(entity))
        return eventDeclaration(entity);
    else if (isField(entity))
        return fieldDeclaration(entity);
}

function typeDeclaration(context: FormatContext, entity: ITypeEntity, typeKeyword: string): ReactFragment {
    return [
        entity.b ? entity.b.map(x => [attribute(context, x), <br/>]) : null,
        accessibility(entity.a),
        modifiers(entity.m),
        keyword(typeKeyword), ' ', nameWithGenericParameters(context, entity),
        entity.t ? [': ', join(entity.t.map(x => typeReference(context, x)), ', ')] : null,
        entity.g ? entity.g.map(x => genericParameterConstraint(x)) : null
    ];
}
