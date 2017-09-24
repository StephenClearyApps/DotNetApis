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
}

export interface IStructGenericConstraint extends IGenericConstraintBase {
}

export interface INewGenericConstraint extends IGenericConstraintBase {
}

export interface ITypeGenericConstraint extends IGenericConstraintBase {
    t: ITypeReference; // The type.
}

export type IGenericConstraint = IClassGenericConstraint | IStructGenericConstraint | INewGenericConstraint | ITypeGenericConstraint;

export function isClass(genericConstraint: IGenericConstraintBase): genericConstraint is IClassGenericConstraint {
    return !genericConstraint.k;
}

export function isStruct(genericConstraint: IGenericConstraintBase): genericConstraint is IStructGenericConstraint {
    return genericConstraint.k === GenericConstraintKind.STRUCT;
}

export function isNew(genericConstraint: IGenericConstraintBase): genericConstraint is INewGenericConstraint {
    return genericConstraint.k === GenericConstraintKind.NEW;
}

export function isType(genericConstraint: IGenericConstraintBase): genericConstraint is ITypeGenericConstraint {
    return genericConstraint.k === GenericConstraintKind.TYPE;
}
