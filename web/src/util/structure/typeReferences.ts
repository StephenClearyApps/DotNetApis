import { ILocation } from './locations';

// This must be kept in sync with DotNetApis.Structure.TypeReferences.TypeReferenceKind
export const enum TypeReferenceKind {
    TYPE = 0,
    KEYWORD = 1,
    GENERIC_INSTANCE = 2,
    DYNAMIC = 3,
    GENERIC_PARAMETER = 4,
    ARRAY = 5,
    REQMOD = 6,
    POINTER = 7
};

export interface ITypeReferenceBase {
    k?: TypeReferenceKind;
}

export interface ISimpleOrOpenGenericTypeReference extends ITypeReferenceBase {
    /** The simple name of the type. */
    n: string;

    /** The namespace. Only present if this is a top-level type. */
    s?: string;

    /** The declaring type. Only present if this is a nested type. */
    t?: ITypeReference;

    /** The location of the type. */
    l: ILocation;

    /** Number of generic arguments. Only present if this is an open generic type. */
    a?: number;
}

export interface IKeywordTypeReference extends ITypeReferenceBase {
    /** The keyword. */
    n: string;

    /** The location of the type for this keyword. */
    l: ILocation;
}

export interface IGenericInstanceTypeReference extends ITypeReferenceBase {
    /** Full child types. */
    t: Array<IConcreteTypeReference>;
}

export interface IDynamicTypeReference extends ITypeReferenceBase {
}

export interface IGenericParameterTypeReference extends ITypeReferenceBase {
    /** The name of the generic parameter. */
    n: string;
}

export interface IArrayTypeReference extends ITypeReferenceBase {
    /** The element type. */
    t: ITypeReference;

    /** The array dimensions. */
    d?: Array<IArrayDimension>;
}

export interface IRequiredModifierTypeReference extends ITypeReferenceBase {
    /** The location of the reqmod type. */
    l: ILocation;

    /** The child type. */
    t: ITypeReference;
}

export interface IPointerTypeReference extends ITypeReferenceBase {
    /** The child type. */
    t: ITypeReference;
}

export type ITypeReference = ISimpleOrOpenGenericTypeReference | IKeywordTypeReference | IGenericInstanceTypeReference | IDynamicTypeReference |
    IGenericParameterTypeReference | IArrayTypeReference | IRequiredModifierTypeReference | IPointerTypeReference;

export function isSimpleOrOpenGeneric(typeReference: ITypeReferenceBase): typeReference is ISimpleOrOpenGenericTypeReference {
    return !typeReference.k;
}

export function isKeyword(typeReference: ITypeReferenceBase): typeReference is IKeywordTypeReference {
    return typeReference.k === TypeReferenceKind.KEYWORD;
}

export function isGenericInstance(typeReference: ITypeReferenceBase): typeReference is IGenericInstanceTypeReference {
    return typeReference.k === TypeReferenceKind.GENERIC_INSTANCE;
}

export function isDynamic(typeReference: ITypeReferenceBase): typeReference is IDynamicTypeReference {
    return typeReference.k === TypeReferenceKind.DYNAMIC;
}

export function isGenericParameter(typeReference: ITypeReferenceBase): typeReference is IGenericParameterTypeReference {
    return typeReference.k === TypeReferenceKind.GENERIC_PARAMETER;
}

export function isArray(typeReference: ITypeReferenceBase): typeReference is IArrayTypeReference {
    return typeReference.k === TypeReferenceKind.ARRAY;
}

export function isRequiredModifier(typeReference: ITypeReferenceBase): typeReference is IRequiredModifierTypeReference {
    return typeReference.k === TypeReferenceKind.REQMOD;
}

export function isPointer(typeReference: ITypeReferenceBase): typeReference is IPointerTypeReference {
    return typeReference.k === TypeReferenceKind.POINTER;
}

export interface IConcreteTypeReference {
    /** The simple name of the type. */
    n: string;

    /** The location of the type reference. */
    l: ILocation;

    /** The types passed as generic arguments to this type. */
    a?: Array<ITypeReference>;
}

export interface IArrayDimension {
    /** Upper bound. */
    u?: number;
}
