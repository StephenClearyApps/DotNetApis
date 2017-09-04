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
    public abstract class AssemblyBase : IAssembly
    {
        private readonly Lazy<AssemblyDefinition> _assemblyDefinition;
        private readonly Lazy<Dictionary<string, FriendlyName>> _dnaIdToFriendlyName;

        protected AssemblyBase(ILogger logger, string path, ReaderParameters readerParameters, IDictionary<string, string> xmldocIdToDnaId)
        {
            Name = Path.GetFileNameWithoutExtension(path);
            _assemblyDefinition = new Lazy<AssemblyDefinition>(() =>
            {
                try
                {
                    // Cecil requires a seekable stream to read the file correctly.
                    var stream = new MemoryStream();
                    Read().CopyTo(stream);
                    stream.Position = 0;
                    return AssemblyDefinition.ReadAssembly(stream, readerParameters);
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

        protected abstract Stream Read();

        protected abstract ILocation Location(string dotNetApiIdentifier);

        public string Name { get; }

        public AssemblyDefinition AssemblyDefinition => _assemblyDefinition.Value;

        public (ILocation, FriendlyName)? TryGetDnaIdLocationAndFriendlyName(string dnaId)
        {
            if (!_assemblyDefinition.IsValueCreated || _dnaIdToFriendlyName.Value == null || !_dnaIdToFriendlyName.Value.ContainsKey(dnaId))
                return null;
            return (Location(dnaId), _dnaIdToFriendlyName.Value[dnaId]);
        }
    }
}
