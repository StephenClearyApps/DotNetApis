import { IEntityBase, IEntity, isType, ITypeEntity, IPackage, IPackageDependency, IAssembly } from '../structure';

const entityCollections = ['l', 's', 'i', 't'];

function find(entity: IEntityBase, i: string): IEntity {
    if (entity.i === i)
        return entity;

    if (isType(entity) && entity.e) {
        for (let name of entityCollections) {
            if (entity.e[name]) {
                for (let nestedEntity of entity.e[name]) {
                    const result = find(nestedEntity, i);
                    if (result)
                        return result;
                }
            }
        }
    }
    return undefined;
}

function findParent(entity: IEntityBase, i: string): ITypeEntity {
    if (isType(entity) && entity.e) {
        for (let name of entityCollections) {
            if (entity.e[name]) {
                for (let nestedEntity of entity.e[name]) {
                    if (nestedEntity.i === i)
                        return entity;
                    const result = findParent(nestedEntity, i);
                    if (result)
                        return result;
                }
            }
        }
    }
    return undefined;
}

export class PackageDoc implements IPackage {
    i: string; // Package ID
    v: string; // Version
    t: string; // Platform target
    d: string; // Description / Summary
    a: string[]; // Authors
    c: string; // Icon URL
    p: string; // Project URL
    f: string[]; // All supported platform targets
    e: IPackageDependency[]; // Dependencies
    b: string; // Publication date
    r: boolean; // Version is a release version (not pre-release).
    l: IAssembly[]; // .NET files

    findEntity(i: string): IEntity {
        for (let dll of this.l) {
            for (let type of dll.t) {
                const result = find(type, i);
                if (result)
                    return result;
            }
        }
        return undefined;
    }

    findEntityParent(i: string): ITypeEntity {
        for (let dll of this.l) {
            for (let type of dll.t) {
                if (type.i === i)
                    return;
                const result = findParent(type, i);
                if (result)
                    return result;
            }
        }
        return undefined;
    }

    findEntityAssembly(i: string): IAssembly {
        for (let assembly of this.l) {
            for (let type of assembly.t) {
                if (find(type, i))
                    return assembly;
            }
        }
        return undefined;
    }

    static create(data: IPackage): PackageDoc {
        const result = Object.assign(new PackageDoc(), data);
        return result;
    }
}