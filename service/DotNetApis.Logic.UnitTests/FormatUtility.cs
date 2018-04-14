using DotNetApis.Cecil;
using DotNetApis.Logic;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Logic.Formatting;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using DotNetApis.Structure;
using DotNetApis.Structure.Xmldoc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public static class FormatUtility
{
    public static AssemblyJson Format(string code)
    {
        var container = new Container();
        container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        container.Options.DefaultLifestyle = Lifestyle.Scoped;
        container.RegisterInstance<ILogger>(NullLogger.Instance);
        container.RegisterInstance<IReferenceXmldocTable>(new NullReferenceXmldocTable());
        container.Register<AttributeFormatter>();
        container.Register<MemberDefinitionFormatter>();
        container.Verify();

        var (dll, xml) = Utility.Compile(code);
        using (AsyncScopedLifestyle.BeginScope(container))
        using (GenerationScope.Create(PlatformTarget.TryParse("net46"), new AssemblyCollection(NullLogger.Instance, null)))
        using (AssemblyScope.Create(xml))
        {
            var attributeFormatter = container.GetInstance<AttributeFormatter>();
            var memberDefinitionFormatter = container.GetInstance<MemberDefinitionFormatter>();
            return new AssemblyJson
            {
                Attributes = attributeFormatter.Attributes(dll, "assembly").ToList(),
                Types = dll.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()).Select(x => memberDefinitionFormatter.MemberDefinition(x)).ToList(),
            };
        }
    }

    private sealed class NullReferenceXmldocTable : IReferenceXmldocTable
    {
        public IBatch CreateBatch() => throw new NotImplementedException();

        public IBatchAction CreateSetRecordAction(PlatformTarget framework, string xmldocId, ReferenceXmldocTableRecord record) => throw new NotImplementedException();

        public ReferenceXmldocTableRecord? TryGetRecord(PlatformTarget framework, string xmldocId) => null;
    }

    public static string ToXmlString(this IXmldocNode xmldocNode)
    {
        if (xmldocNode is StringXmldocNode stringXmldocNode)
            return stringXmldocNode.Text.Trim();
        if (xmldocNode is XmlXmldocNode xmlXmldocNode)
        {
            var xml = new XElement(xmlXmldocNode.Kind.ToString(), string.Join("", xmlXmldocNode.Children.Select(x => x.ToXmlString())));
            foreach (var attribute in xmlXmldocNode.Attributes?.GetType()?.GetProperties() ?? Enumerable.Empty<PropertyInfo>())
                xml.SetAttributeValue(attribute.Name, attribute.GetValue(xmlXmldocNode.Attributes));
            return xml.ToString(SaveOptions.DisableFormatting);
        }
        throw new InvalidOperationException("Unknown IXmldocNode type " + xmldocNode.GetType().Name);
    }
}