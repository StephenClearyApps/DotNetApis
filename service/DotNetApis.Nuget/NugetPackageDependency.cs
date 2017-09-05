namespace DotNetApis.Nuget
{
    public sealed class NugetPackageDependency
    {
        public NugetPackageDependency(string packageId, NugetVersionRange versionRange)
        {
            PackageId = packageId;
            VersionRange = versionRange;
        }

        public string PackageId { get; }
        public NugetVersionRange VersionRange { get; }

        public override string ToString() => $"{PackageId} {VersionRange}";
    }
}
