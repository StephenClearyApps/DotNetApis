export function packageEntityLink(packageId: string, packageVersion: string, targetFramework: string, dnaid: string) {
    return '/pkg/' + packageId + '/' + (packageVersion || '$') + '/' + (targetFramework || '$') + '/doc/' + dnaid;
}