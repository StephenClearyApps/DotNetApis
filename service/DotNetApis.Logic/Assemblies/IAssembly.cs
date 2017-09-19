using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Structure;
using DotNetApis.Structure.Locations;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// An assembly that can be loaded into memory.
    /// </summary>
    public interface IAssembly
    {
        /// <summary>
        /// The filename of the assembly, without extension.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The assembly as loaded by Cecil. This property is lazily evaluated, and may be <c>null</c> if there was a problem loading or processing the assembly.
        /// </summary>
        AssemblyDefinition AssemblyDefinition { get; }

        /// <summary>
        /// Looks up a dnaid in this assembly and returns its location and friendly name. If the assembly has not been loaded yet, returns <c>null</c>.
        /// </summary>
        /// <param name="dnaid">The dnaid</param>
        (ILocation Location, FriendlyName FriendlyName)? TryGetDnaIdLocationAndFriendlyName(string dnaid);
    }
}
