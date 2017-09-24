import { IAttribute } from './attributes';
import { IEntity } from './entities';

/** A package that is a dependency for a primary package */
export interface IPackageDependency {
    /** Title */
    t: string;

    /** Package ID */
    i: string;

    /** Version range requested by the primary package (optional) */
    q?: string;

    /** Resolved version */
    v: string;

    /** Summary or description (optional) */
    d?: string;

    /** Authors (optional) */
    a?: string[];

    /** Icon URL (optional) */
    c?: string;

    /** Project URL (optional) */
    p?: string;
}

/** A dll or other .NET binary file in a package */
export interface IAssembly {
    /** The assembly full name */
    n: string;

    /** The path of this file within the nupkg (when treated as a zip file) */
    p: string;

    /** The size of the file, in bytes. */
    s: number;

    /** Assembly-level attributes (optional) */
    b?: IAttribute[];

    /** Top-level entities in this assembly */
    t: IEntity[];
}

/** A NuGet package, applied to a particular target framework. */
export interface IPackage {
    /** Package ID */
    i: string;

    /** Package Version */
    v: string;

    /** Platform target */
    t: string;

    /** Description / Summary (optional) */
    d?: string;

    /** Authors (optional) */
    a?: string[];

    /** Icon URL (optional) */
    c?: string;

    /** Project URL (optional) */
    p?: string;

    /** All supported platform targets */
    f: string[];

    /** Dependencies (optional) */
    e?: IPackageDependency[];

    /** Publication date */
    b: string;

    /** Whether this version is a release version (not pre-release). */
    r?: boolean;

    /** Assemblies in this package (optional). */
    l?: IAssembly[];
}