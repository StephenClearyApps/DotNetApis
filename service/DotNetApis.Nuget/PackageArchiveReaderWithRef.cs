using System.Collections.Generic;
using System.IO;
using NuGet.Packaging;

namespace DotNetApis.Nuget
{
    /// <summary>
    /// A package archive reader that understands the /ref folders.
    /// </summary>
    public sealed class PackageArchiveReaderWithRef : PackageArchiveReader, IPackageContentReaderWithRef
    {
        public PackageArchiveReaderWithRef(Stream stream, bool leaveStreamOpen)
            : base(stream, leaveStreamOpen)
        {
        }

        public IEnumerable<FrameworkSpecificGroup> GetRefItems() => GetFileGroups(PackagingConstants.Folders.Ref);
    }
}
