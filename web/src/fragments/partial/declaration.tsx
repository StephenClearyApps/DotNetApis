import { IEntity, isClass, isInterface, isStruct, isEnum, isDelegate, isMethod, isProperty, isEvent, isField } from "../../structure";
import { FormatContext, Styles, ReactFragment, typeDeclaration, enumDeclaration, delegateDeclaration, methodDeclaration, propertyDeclaration, eventDeclaration, fieldDeclaration } from ".";

export function declaration(context: FormatContext, entity: IEntity, ns?: string): ReactFragment {
    if (isClass(entity))
        return typeDeclaration(context, entity, context.style === Styles.TITLE ? "Class" : "class", ns);
    else if (isInterface(entity))
        return typeDeclaration(context, entity, context.style === Styles.TITLE ? "Interface" : "interface", ns);
    else if (isStruct(entity))
        return typeDeclaration(context, entity, context.style === Styles.TITLE ? "Struct" : "struct", ns);
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