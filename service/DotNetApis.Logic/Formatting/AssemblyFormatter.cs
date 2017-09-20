using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Structure;

namespace DotNetApis.Logic.Formatting
{
    public sealed class AssemblyFormatter
    {
        private readonly AttributeFormatter _attributeFormatter;
        private readonly MemberDefinitionFormatter _memberDefinitionFormatter;

        public AssemblyFormatter(AttributeFormatter attributeFormatter, MemberDefinitionFormatter memberDefinitionFormatter)
        {
            _attributeFormatter = attributeFormatter;
            _memberDefinitionFormatter = memberDefinitionFormatter;
        }

        public AssemblyJson Assembly(CurrentPackageAssembly assembly)
        {
            using (AssemblyScope.Create(assembly.Xmldoc))
            {
                return new AssemblyJson
                {
                    FullName = assembly.AssemblyDefinition.FullName,
                    Path = assembly.Path,
                    FileLength = assembly.FileLength,
                    Attributes = _attributeFormatter.Attributes(assembly.AssemblyDefinition, "assembly").ToList(),
                    Types = assembly.AssemblyDefinition.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()).Select(x => _memberDefinitionFormatter.MemberDefinition(x, assembly.Xmldoc)).ToList(),
                };
            }
        }
    }
}
