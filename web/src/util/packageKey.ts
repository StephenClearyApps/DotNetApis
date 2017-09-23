export function packageKey(elements: PackageKey): string {
    const { packageId, packageVersion, targetFramework } = elements;
    return packageId + '/' + (packageVersion || '$') + '/' + (targetFramework || '$');
}
