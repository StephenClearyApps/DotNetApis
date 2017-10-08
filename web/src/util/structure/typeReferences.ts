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

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __ISimpleOrOpenGenericTypeReference: undefined;
}

export interface IKeywordTypeReference extends ITypeReferenceBase {
    /** The keyword. */
    n: string;

    /** The location of the type for this keyword. */
    l: ILocation;

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IKeywordTypeReference: undefined;
}

export interface IGenericInstanceTypeReference extends ITypeReferenceBase {
    /** Full child types. */
    t: Array<IConcreteTypeReference>;

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IGenericInstanceTypeReference: undefined;
}

export interface IDynamicTypeReference extends ITypeReferenceBase {
    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IDynamicTypeReference: undefined;
}

export interface IGenericParameterTypeReference extends ITypeReferenceBase {
    /** The name of the generic parameter. */
    n: string;

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IGenericParameterTypeReference: undefined;
}

export interface IArrayTypeReference extends ITypeReferenceBase {
    /** The element type. */
    t: ITypeReference;

    /** The array dimensions. */
    d?: Array<IArrayDimension>;

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IArrayTypeReference: undefined;
}

export interface IRequiredModifierTypeReference extends ITypeReferenceBase {
    /** The location of the reqmod type. */
    l: ILocation;

    /** The child type. */
    t: ITypeReference;

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IRequiredModifierTypeReference: undefined;
}

export interface IPointerTypeReference extends ITypeReferenceBase {
    /** The child type. */
    t: ITypeReference;

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IPointerTypeReference: undefined;
}

export type ITypeReference = ISimpleOrOpenGenericTypeReference | IKeywordTypeReference | IGenericInstanceTypeReference | IDynamicTypeReference |
    IGenericParameterTypeReference | IArrayTypeReference | IRequiredModifierTypeReference | IPointerTypeReference;

export function isSimpleOrOpenGeneric(typeReference: ITypeReference): typeReference is ISimpleOrOpenGenericTypeReference {
    return !typeReference.k;
}

export function isKeyword(typeReference: ITypeReference): typeReference is IKeywordTypeReference {
    return typeReference.k === TypeReferenceKind.KEYWORD;
}

export function isGenericInstance(typeReference: ITypeReference): typeReference is IGenericInstanceTypeReference {
    return typeReference.k === TypeReferenceKind.GENERIC_INSTANCE;
}

export function isDynamic(typeReference: ITypeReference): typeReference is IDynamicTypeReference {
    return typeReference.k === TypeReferenceKind.DYNAMIC;
}

export function isGenericParameter(typeReference: ITypeReference): typeReference is IGenericParameterTypeReference {
    return typeReference.k === TypeReferenceKind.GENERIC_PARAMETER;
}

export function isArray(typeReference: ITypeReference): typeReference is IArrayTypeReference {
    return typeReference.k === TypeReferenceKind.ARRAY;
}

export function isRequiredModifier(typeReference: ITypeReference): typeReference is IRequiredModifierTypeReference {
    return typeReference.k === TypeReferenceKind.REQMOD;
}

export function isPointer(typeReference: ITypeReference): typeReference is IPointerTypeReference {
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
