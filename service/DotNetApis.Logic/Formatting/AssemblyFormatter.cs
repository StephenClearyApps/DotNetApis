using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Common;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic.Formatting
{
    public sealed class AssemblyFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly MemberDefinitionFormatter _memberDefinitionFormatter;
        private readonly ILogger<AssemblyFormatter> _logger;

        public AssemblyFormatter(AttributeFormatter attributeFormatter, MemberDefinitionFormatter memberDefinitionFormatter, ILoggerFactory loggerFactory)
        {
            _attributeFormatter = attributeFormatter;
            _memberDefinitionFormatter = memberDefinitionFormatter;
            _logger = loggerFactory.CreateLogger<AssemblyFormatter>();
        }

        /// <summary>
        /// Formats an assembly. This method establishes its own <see cref="AssemblyScope"/>.
        /// </summary>
        /// <param name="assembly">The assembly to format.</param>
        /// <param name="doc">The string JSON documentation.</param>
        public void Assembly(CurrentPackageAssembly assembly, StreamingJsonWriter doc)
        {
            _logger.ProcessingAssembly(assembly.Path);
            using (AssemblyScope.Create(assembly.Xmldoc))
            {
                var stopwatch = Stopwatch.StartNew();
                doc.WriteStartObject();
                doc.WriteProperty("n", assembly.AssemblyDefinition.FullName);
                doc.WriteProperty("p", assembly.Path);
                doc.WriteProperty("s", assembly.FileLength);
                doc.WriteProperty("b", _attributeFormatter.Attributes(assembly.AssemblyDefinition, "assembly").ToList());
                doc.WritePropertyName("t");
                doc.WriteStartArray();
                foreach (var t in assembly.AssemblyDefinition.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()))
                    _memberDefinitionFormatter.MemberDefinition(t, doc);
                doc.WriteEndArray();
                doc.WriteEndObject();
                _logger.ProcessedAssembly(assembly.Path, stopwatch.Elapsed);
            }
        }
    }

    internal static partial class Logging
    {
        public static void ProcessingAssembly(this ILogger<AssemblyFormatter> logger, string path) =>
            Logger.Log(logger, 1, LogLevel.Information, "Processing {path}", path, null);

        public static void ProcessedAssembly(this ILogger<AssemblyFormatter> logger, string path, TimeSpan elapsed) =>
            Logger.Log(logger, 2, LogLevel.Information, "Processed {path} in {elapsed}", path, elapsed, null);
    }
}
