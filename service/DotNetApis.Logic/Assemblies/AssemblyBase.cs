using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// A base type for assemblies that can be read by streams, providing lazy loading for both the assembly and its dnaid lookups.
    /// </summary>
    public abstract class AssemblyBase : IAssembly
    {
        private readonly Lazy<AssemblyDefinition> _assemblyDefinition;
        private readonly Lazy<Dictionary<string, FriendlyName>> _dnaIdToFriendlyName;

        /// <summary>
        /// Initializes the base type.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="path">The path of the assembly. This can include path segments, the file name, and the extension.</param>
        /// <param name="readerParameters">The parameters used when processing the assembly by Cecil.</param>
        /// <param name="xmldocIdToDnaId">A reference to the shared xmldoc to dnaid mapping, which is updated when the assembly is processed.</param>
        protected AssemblyBase(ILogger logger, string path, ReaderParameters readerParameters, IDictionary<string, string> xmldocIdToDnaId)
        {
            Name = Path.GetFileNameWithoutExtension(path);
            _assemblyDefinition = new Lazy<AssemblyDefinition>(() =>
            {
                try
                {
                    // Cecil requires a seekable stream to read the file correctly.
                    var source = Read();
                    if (!source.CanSeek)
                    {
                        var stream = new MemoryStream();
                        source.CopyTo(stream);
                        stream.Position = 0;
                        source = stream;
                    }
                    return AssemblyDefinition.ReadAssembly(source, readerParameters);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(0, ex, "Unable to load assembly {type} from {path}", GetType().Name, path);
                    return null;
                }
            });
            _dnaIdToFriendlyName = new Lazy<Dictionary<string, FriendlyName>>(() =>
            {
                try
                {
                    var result = new Dictionary<string, FriendlyName>();
                    if (AssemblyDefinition == null)
                        return null;
                    new AssemblyIndexer(result, xmldocIdToDnaId).AddExposed(AssemblyDefinition);
                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(0, ex, "Unable to process assembly {type} from {path}", GetType().Name, path);
                    return null;
                }
            });
        }

        /// <summary>
        /// Reads the assembly binary as a stream.
        /// </summary>
        protected abstract Stream Read();

        /// <summary>
        /// Get a location structure for a given dnaid.
        /// </summary>
        /// <param name="dnaid">The dnaid to locate.</param>
        protected abstract ILocation Location(string dnaid);

        public string Name { get; }

        public AssemblyDefinition AssemblyDefinition => _assemblyDefinition.Value;

        public (ILocation Location, FriendlyName FriendlyName)? TryGetDnaIdLocationAndFriendlyName(string dnaId)
        {
            // If the assembly has not been demand-loaded yet, return null.
            if (!_assemblyDefinition.IsValueCreated)
                return null;

            // If there was a problem loading the assembly, return null.
            if (_dnaIdToFriendlyName.Value == null)
                return null;

            return _dnaIdToFriendlyName.Value.ContainsKey(dnaId) ? (Location(dnaId), _dnaIdToFriendlyName.Value[dnaId]) : ((ILocation, FriendlyName)?)null;
        }
    }
}
