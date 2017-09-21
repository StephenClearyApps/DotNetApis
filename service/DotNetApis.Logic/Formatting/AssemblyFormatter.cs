using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic.Formatting
{
    public sealed class AssemblyFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly MemberDefinitionFormatter _memberDefinitionFormatter;
        private readonly ILogger _logger;

        public AssemblyFormatter(AttributeFormatter attributeFormatter, MemberDefinitionFormatter memberDefinitionFormatter, ILogger logger)
        {
            _attributeFormatter = attributeFormatter;
            _memberDefinitionFormatter = memberDefinitionFormatter;
            _logger = logger;
        }

        /// <summary>
        /// Formats an assembly. This method establishes its own <see cref="AssemblyScope"/>.
        /// </summary>
        /// <param name="assembly">The assembly to format.</param>
        public AssemblyJson Assembly(CurrentPackageAssembly assembly)
        {
            _logger.LogInformation("Processing {assembly}", assembly.Path);
            using (AssemblyScope.Create(assembly.Xmldoc))
            {
                var stopwatch = Stopwatch.StartNew();
                var result = new AssemblyJson
                {
                    FullName = assembly.AssemblyDefinition.FullName,
                    Path = assembly.Path,
                    FileLength = assembly.FileLength,
                    Attributes = _attributeFormatter.Attributes(assembly.AssemblyDefinition, "assembly").ToList(),
                    Types = assembly.AssemblyDefinition.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()).Select(x => _memberDefinitionFormatter.MemberDefinition(x)).ToList(),
                };
                _logger.LogInformation("Processed {assembly} in {elapsed}", assembly.Path, stopwatch.Elapsed);
                return result;
            }
        }
    }
}
