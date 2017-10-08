/** A location in a package that is a dependency for the current package. */
export interface IDependencyLocation {
    /** Package ID */
    p: string;

    /** Package version */
    v: string;

    /** DnaID */
    i: string;
}

/** A location in a reference dll. */
export interface IReferenceLocation {
    /** DnaID */
    i: string;
}

/** A location within the current package (though possibly in a different dll). */
export type ICurrentPackageLocation = string;

/** The location of an entity. */
export type ILocation = ICurrentPackageLocation | IDependencyLocation | IReferenceLocation;

export function isCurrentPackageLocation(location: ILocation): location is ICurrentPackageLocation {
    return typeof (location) === 'string';
}

export function isDependencyLocation(location: ILocation): location is IDependencyLocation {
    return !isCurrentPackageLocation(location) && (location as any).p;
}

export function isReferenceLocation(location: ILocation): location is IReferenceLocation {
    return !isCurrentPackageLocation(location) && !(location as any).p;
}