using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure;
using DotNetApis.Structure.Locations;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// A platform reference assembly.
    /// </summary>
    public sealed class ReferenceAssembly : AssemblyBase
    {
        private readonly Func<Stream> _read;

        /// <summary>
        /// Initializes the assembly.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="path">The path of the assembly. This can include path segments, the file name, and the extension.</param>
        /// <param name="readerParameters">The parameters used when processing the assembly by Cecil.</param>
        /// <param name="xmldocIdToDnaId">A reference to the shared xmldoc to dnaid mapping, which is updated when the assembly is processed.</param>
        /// <param name="read">A function that reads the assembly as a stream. Must not be <c>null</c>.</param>
        public ReferenceAssembly(ILogger logger, string path, ReaderParameters readerParameters, IDictionary<string, string> xmldocIdToDnaId, Func<Stream> read)
            : base(logger, path, readerParameters, xmldocIdToDnaId)
        {
            _read = read;
        }

        protected override ILocation Location(string dnaid) => new ReferenceLocation
        {
            DnaId = dnaid,
        };

        protected override Stream Read() => _read();
    }
}
