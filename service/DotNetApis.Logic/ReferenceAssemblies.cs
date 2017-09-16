using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic
{
    /// <summary>
    /// Maintains a list of all known reference assemblies, organized by target framework.
    /// </summary>
    public sealed class ReferenceAssemblies
    {
        private ReferenceAssemblies()
        {
        }

        /// <summary>
        /// The list of platforms along with their reference assemblies.
        /// </summary>
        public IReadOnlyList<ReferenceTarget> ReferenceTargets { get; private set; }

        public static async Task<ReferenceAssemblies> CreateAsync(IReferenceStorage referenceStorage)
        {
            var folders = await referenceStorage.GetFoldersAsync().ConfigureAwait(false);
            var targets = await Task.WhenAll(folders.Select(x => ReferenceTarget.CreateAsync(x, referenceStorage))).ConfigureAwait(false);
            return new ReferenceAssemblies
            {
                ReferenceTargets = targets.ToList(),
            };
        }

        /// <summary>
        /// A specific patform target along with paths of all its reference assemblies.
        /// </summary>
        public sealed class ReferenceTarget
        {
            /// <summary>
            /// The target these reference assemblies are for. Never <c>null</c>.
            /// </summary>
            public PlatformTarget Target { get; private set; }

            /// <summary>
            /// The paths of all files related to this target. This includes .dll and .xml files. Never <c>null</c>.
            /// </summary>
            public IReadOnlyList<string> Paths { get; private set; }

            public static async Task<ReferenceTarget> CreateAsync(string path, IReferenceStorage referenceStorage)
            {
                var target = PlatformTarget.TryParse(path.TrimEnd('/'));
                if (target == null)
                    throw new InvalidOperationException($"Unrecognized reference target framework {path}");

                return new ReferenceTarget
                {
                    Target = target,
                    Paths = await referenceStorage.GetFilesAsync(path).ConfigureAwait(false),
                };
            }

            public override string ToString() => Target.ToString();
        }
    }
}
