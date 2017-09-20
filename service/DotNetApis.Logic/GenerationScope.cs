using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Nuget;

namespace DotNetApis.Logic
{
    public sealed class GenerationScope : ScopeBase<GenerationScope>
    {
        private GenerationScope(PlatformTarget platformTarget, AssemblyCollection asssemblies)
        {
            PlatformTarget = platformTarget;
            Asssemblies = asssemblies;
        }

        public PlatformTarget PlatformTarget { get; }
        public AssemblyCollection Asssemblies { get; }

        public static IDisposable Create(PlatformTarget platformTarget, AssemblyCollection asssemblies) => Create(new GenerationScope(platformTarget, asssemblies));
    }
}
