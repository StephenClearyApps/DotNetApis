using System.Collections.Generic;
using System.Linq;
using NuGet.Frameworks;
using NuGet.Packaging;

namespace DotNetApis.Nuget
{
    /// <summary>
    /// A package content reader that also understands the /ref folder.
    /// </summary>
    public interface IPackageContentReaderWithRef : IPackageContentReader
    {
        IEnumerable<FrameworkSpecificGroup> GetRefItems();
    }

    public static class PackageContentReaderWithRefExtensions
    {
        /// <summary>
        /// Gets all frameworks supported by this package, including frameworks in the /ref folder.
        /// </summary>
        /// <typeparam name="T">The type of the package reader.</typeparam>
        /// <param name="packageReader">The package reader.</param>
        public static IEnumerable<NuGetFramework> GetSupportedFrameworksWithRef<T>(this T packageReader)
            where T : PackageReaderBase, IPackageContentReaderWithRef
        {
            var frameworks = new HashSet<NuGetFramework>(new NuGetFrameworkFullComparer());
            frameworks.UnionWith(packageReader.GetSupportedFrameworks());
            frameworks.UnionWith(packageReader.GetRefItems().Select(x => x.TargetFramework).Where(x => !x.IsUnsupported));
            return frameworks.OrderBy(x => x, new NuGetFrameworkSorter());
        }
    }
}
