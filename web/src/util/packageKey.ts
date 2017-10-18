export function with$({ packageId, packageVersion, targetFramework }: PackageKey): PackageKey {
    return { packageId, packageVersion: packageVersion || '$', targetFramework: targetFramework || '$' };
}

export function without$({ packageId, packageVersion, targetFramework }: PackageKey): PackageKey {
    return { packageId, packageVersion: packageVersion === '$' ? undefined : packageVersion, targetFramework: targetFramework === '$' ? undefined : targetFramework };
}

export function packageKey(packageKey: PackageKey): string {
    const k = with$(packageKey);
    return k.packageId + '/' + k.packageVersion + '/' + k.targetFramework;
}

export function packageFriendlyName(packageKey: PackageKey): string {
    const { packageId, packageVersion, targetFramework } = without$(packageKey);
    let result = packageId;
    if (packageVersion) {
        result += " " + packageVersion;
    }
    if (targetFramework) {
        result += " (" + targetFramework + ")";
    }
    return result;
}