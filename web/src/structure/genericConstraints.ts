import { ITypeReference } from './typeReferences';
import { IXmldoc } from './xmldoc';

// This must be kept in sync with DotNetApis.Structure.GenericConstraints.GenericConstraintKind
export const enum GenericConstraintKind {
    CLASS = 0,
    STRUCT = 1,
    NEW = 2,
    TYPE = 3
};

export interface IGenericConstraintBase {
    k?: GenericConstraintKind;
}

export interface IClassGenericConstraint extends IGenericConstraintBase {
    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IClassGenericConstraint: undefined;
}

export interface IStructGenericConstraint extends IGenericConstraintBase {
    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __IStructGenericConstraint: undefined;
}

export interface INewGenericConstraint extends IGenericConstraintBase {
    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __INewGenericConstraint: undefined;
}

export interface ITypeGenericConstraint extends IGenericConstraintBase {
    t: ITypeReference; // The type.

    /** https://github.com/Microsoft/TypeScript/issues/13325 */
    __ITypeGenericConstraint: undefined;
}

export type IGenericConstraint = IClassGenericConstraint | IStructGenericConstraint | INewGenericConstraint | ITypeGenericConstraint;

export function isClassGenericConstrint(genericConstraint: IGenericConstraintBase): genericConstraint is IClassGenericConstraint {
    return !genericConstraint.k;
}

export function isStructGenericConstrint(genericConstraint: IGenericConstraintBase): genericConstraint is IStructGenericConstraint {
    return genericConstraint.k === GenericConstraintKind.STRUCT;
}

export function isNewGenericConstrint(genericConstraint: IGenericConstraintBase): genericConstraint is INewGenericConstraint {
    return genericConstraint.k === GenericConstraintKind.NEW;
}

export function isTypeGenericConstrint(genericConstraint: IGenericConstraintBase): genericConstraint is ITypeGenericConstraint {
    return genericConstraint.k === GenericConstraintKind.TYPE;
}
