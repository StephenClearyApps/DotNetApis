import * as entities from './structure/entities';
import * as packages from './structure/packages';

const entityCollections = ['l', 's', 'i', 't'];

function find(entity: entities.IEntityBase, i: string): entities.IEntity {
    if (entity.i === i)
        return entity;

    if (entities.isType(entity) && entity.e) {
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

function findParent(entity: entities.IEntityBase, i: string): entities.ITypeEntity {
    if (entities.isType(entity) && entity.e) {
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

export class PackageDoc implements packages.IPackage {
    i: string; // Package ID
    v: string; // Version
    t: string; // Platform target
    d: string; // Description / Summary
    a: string[]; // Authors
    c: string; // Icon URL
    p: string; // Project URL
    f: string[]; // All supported platform targets
    e: packages.IPackageDependency[]; // Dependencies
    b: string; // Publication date
    r: boolean; // Version is a release version (not pre-release).
    l: packages.IAssembly[]; // .NET files

    findEntity(i: string): entities.IEntity {
        for (let dll of this.l) {
            for (let type of dll.t) {
                const result = find(type, i);
                if (result)
                    return result;
            }
        }
        return undefined;
    }

    findEntityParent(i: string): entities.ITypeEntity {
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

    findEntityAssembly(i: string): packages.IAssembly {
        for (let assembly of this.l) {
            for (let type of assembly.t) {
                if (find(type, i))
                    return assembly;
            }
        }
        return undefined;
    }

    static create(data: packages.IPackage): PackageDoc {
        const result = Object.assign(new PackageDoc(), data);
        return result;
    }
}