import { IAttribute } from './attributes';
import { ITypeReference } from './typeReferences';
import { ILiteral } from './literals';
import { IXmldoc, IXmldocNode } from './xmldoc';
import { IGenericConstraint } from './genericConstraints';

// This must be kept in sync with DotNetApis.Structure.Entities.EntityKind
/** The type of entity */
export const enum EntityKind {
    CLASS = 0,
    INTERFACE = 1,
    STRUCT = 2,
    ENUM = 3,
    DELEGATE = 4,
    METHOD = 5,
    PROPERTY = 6,
    EVENT = 7,
    FIELD = 8
};

export interface IEntityBase {
    /** DnaID */
    i: string;

    /** The kind of entity */
    k?: number;

    /** Attributes */
    b?: Array<IAttribute>;

    /** Name */
    n?: string;

    /** Structured XML documentation */
    x?: IXmldoc;
}

export interface IParameterizedEntityBase extends IEntityBase {
    /** Parameters */
    p?: Array<IParameter>;
}

export interface ICallableEntityBase extends IParameterizedEntityBase {
    /** Return type */
    r: ITypeReference;

    /** Generic parameters */
    g?: Array<IGenericParameter>;
}

export interface ITopLevelEntityBase extends IEntityBase {
    /** Namespace */
    s?: string;
}

// This must be kept in sync with DotNetApis.Structure.Entities.EntityModifiers
/** Modifiers */
export const enum EntityModifiers {
    NONE = 0,
    STATIC = 0x1,
    ABSTRACT = 0x2,
    SEALED = 0x4,
    VIRTUAL = 0x8,
    OVERRIDE = 0x10,
    CONST = 0x20
};

export interface IMemberEntityBase extends IEntityBase {
    /** Modifiers */
    m?: EntityModifiers;
}

export interface IChildEntityBase extends IEntityBase {
    /** Accessibility */
    a?: EntityAccessibility;
}

export interface IOverridableMemberEntityBase extends IMemberEntityBase {
    /** Declaring type. Only present if the method is an explicit implementation of an interface. */
    d?: ITypeReference;
}

export interface ITypeEntity extends IEntityBase, ITopLevelEntityBase, IMemberEntityBase, IChildEntityBase {
    /** Generic parameters */
    g?: Array<IGenericParameter>;

    /** Base types and interfaces */
    t?: Array<ITypeReference>;

    /** Child entities */
    e?: IEntityGrouping;
}

export interface IClassEntity extends ITypeEntity {
}

export interface IStructEntity extends ITypeEntity {
}

export interface IInterfaceEntity extends ITypeEntity {
}

export interface IEnumEntity extends IEntityBase, ITopLevelEntityBase, IChildEntityBase {
    /** DnaID of the underlying type. If not present, underlying type is `int`. */
    u?: string;

    /** 1 to prefer hexadecimal. */
    h?: number;

    /** Defined enum values. */
    f?: Array<IEnumField>;
}

export interface IDelegateEntity extends IEntityBase, ICallableEntityBase, ITopLevelEntityBase, IChildEntityBase {
}

// This must be kept in sync with DotNetApis.Structure.Entities.MethodStyles
export const enum MethodStyles {
    NONE = 0,
    EXTENSION = 1,
    IMPLICIT = 2,
    EXPLICIT = 3,
    OPERATOR = 4
};

export interface IMethodEntity extends IEntityBase, ICallableEntityBase, IOverridableMemberEntityBase, IChildEntityBase {
    /** Method styles */
    s?: MethodStyles;
}

export interface IPropertyEntity extends IParameterizedEntityBase, IOverridableMemberEntityBase, IChildEntityBase {
    /** The type. */
    t: ITypeReference;

    /** The get method */
    g?: IPropertyMethod;

    /** The set method */
    s?: IPropertyMethod;
}

export interface IEventEntity extends IEntityBase, IOverridableMemberEntityBase, IChildEntityBase {
    /** The type. */
    t: ITypeReference;

    /** Add method attributes. */
    p?: Array<IAttribute>;

    /** Remove method attributes. */
    r?: Array<IAttribute>;
}

export interface IFieldEntity extends IEntityBase, IMemberEntityBase, IChildEntityBase {
    /** The type. */
    t: ITypeReference;

    /** The value. */
    v?: ILiteral;
}

export type IEntity = IClassEntity | IStructEntity | IInterfaceEntity | IEnumEntity | IDelegateEntity | IMethodEntity | IPropertyEntity | IEventEntity | IFieldEntity;

export function isClass(entity: IEntityBase): entity is IClassEntity {
    return !entity.k;
}

export function isStruct(entity: IEntityBase): entity is IStructEntity {
    return entity.k === EntityKind.STRUCT;
}

export function isInterface(entity: IEntityBase): entity is IInterfaceEntity {
    return entity.k === EntityKind.INTERFACE;
}

export function isType(entity: IEntityBase): entity is ITypeEntity {
    return isClass(entity) || isStruct(entity) || isInterface(entity);
}

export function isEnum(entity: IEntityBase): entity is IEnumEntity {
    return entity.k === EntityKind.ENUM;
}

export function isDelegate(entity: IEntityBase): entity is IDelegateEntity {
    return entity.k === EntityKind.DELEGATE;
}

export function isMethod(entity: IEntityBase): entity is IMethodEntity {
    return entity.k === EntityKind.METHOD;
}

export function isProperty(entity: IEntityBase): entity is IPropertyEntity {
    return entity.k === EntityKind.PROPERTY;
}

export function isEvent(entity: IEntityBase): entity is IEventEntity {
    return entity.k === EntityKind.EVENT;
}

export function isField(entity: IEntityBase): entity is IFieldEntity {
    return entity.k === EntityKind.FIELD;
}

export interface IEntityGrouping {
    /** Lifetime entities. */
    l?: IEntity[];

    /** Shared/static entities. */
    s?: IEntity[];

    /** Instance entities. */
    i?: IEntity[];

    /** Type entities. */
    t?: IEntity[];

    [groupName: string]: IEntity[];
}

export interface IEnumField {
    /** Name */
    n: string;

    /** Value */
    v: number;
}

// This must be kept in sync with DotNetApis.Structure.Entities.MethodParameterModifiers
export const enum MethodParameterModifiers {
    NONE = 0,
    PARAMS = 1,
    REF = 2,
    OUT = 3
};

export interface IParameter {
    /** Attributes */
    b?: Array<IAttribute>;

    /** Method parameter modifiers */
    m?: MethodParameterModifiers;

    /** Parameter type */
    t: ITypeReference;

    /** Parameter name */
    n: string;

    /** Default value */
    v?: ILiteral;

    /** Structured XML documentation */
    x?: IXmldocNode;
}

// This must be kept in sync with DotNetApis.Structure.Entities.EntityAccessibility
export const enum EntityAccessibility {
    PUBLIC = 0,
    PROTECTED = 1,
    HIDDEN = 2
};

export interface IPropertyMethod {
    /** Attributes */
    b?: Array<IAttribute>;

    /** Accessibility */
    a?: EntityAccessibility;
}


// This must be kept in sync with DotNetApis.Structure.Entities.GenericParameterModifiers
export const enum GenericParameterModifiers {
    INVARIANT = 0,
    IN = 1,
    OUT = 2
};

export interface IGenericParameter {
    /** Generic parameter modifiers */
    m?: GenericParameterModifiers;

    /** The name of the parameter. */
    n: string;

    /** Generic constraints. */
    c?: Array<IGenericConstraint>;

    /** Structured XML documentation */
    x?: IXmldoc;
}
