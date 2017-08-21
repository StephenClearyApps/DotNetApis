using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging;

namespace Nuget
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
