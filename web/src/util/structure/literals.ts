import { ITypeReference } from "./typeReferences";

// This must be kept in sync with DotNetApis.Structure.Literals.LiteralKind
export const enum LiteralKind {
    NULL = 0,
    PRIMITIVE = 1,
    ARRAY = 2,
    TYPEOF = 3,
    ENUM = 4
};

export interface ILiteralBase {
    k?: LiteralKind;
}

export interface IPrimitiveLiteral extends ILiteralBase {
    /** The value. */
    v: string | number;

    /** `1` to prefer hexadecimal. */
    h?: number;
}

export interface IArrayLiteral extends ILiteralBase {
    /** The element type. */
    t: ITypeReference;

    /** The values. */
    v: Array<ILiteral>;
}

export interface ITypeofLiteral extends ILiteralBase {
    /** The type passed to the typeof operator. */
    t: ITypeReference;
}

export interface IEnumLiteral extends ILiteralBase {
    /** The numeric value of the enum constant. */
    v: number;

    /** `1` to prefer hexadecimal. */
    h?: number;

    /** The enum type. */
    t: ITypeReference;

    /** Enum value names. Not present if the value is not an exact match for one (or more) of the enum names. */
    n?: Array<string>;
}

export type ILiteral = IArrayLiteral | ITypeofLiteral | IPrimitiveLiteral | IEnumLiteral;

export function isNull(literal: ILiteralBase): boolean {
    return !literal.k;
}

export function isPrimitiveLiteral(literal: ILiteralBase): literal is IPrimitiveLiteral {
    return literal.k === LiteralKind.PRIMITIVE;
}

export function isArrayLiteral(literal: ILiteralBase): literal is IArrayLiteral {
    return literal.k === LiteralKind.ARRAY;
}

export function isTypeofLiteral(literal: ILiteralBase): literal is ITypeofLiteral {
    return literal.k === LiteralKind.TYPEOF;
}

export function isEnumLiteral(literal: ILiteralBase): literal is IEnumLiteral {
    return literal.k === LiteralKind.ENUM;
}