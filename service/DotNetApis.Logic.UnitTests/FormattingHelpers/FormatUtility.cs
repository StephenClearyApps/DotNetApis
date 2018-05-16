using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Logic.Formatting;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using DotNetApis.Structure;
using DotNetApis.Structure.Entities;
using DotNetApis.Structure.Xmldoc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DotNetApis.Logic.UnitTests.FormattingHelpers
{
    public static class FormatUtility
    {
        static FormatUtility()
        {
            container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.DefaultLifestyle = Lifestyle.Scoped;
            container.RegisterInstance<ILoggerFactory>(new LoggerFactory());
            container.RegisterInstance<IReferenceXmldocTable>(new NullReferenceXmldocTable());
            container.Register<AttributeFormatter>();
            container.Register<MemberDefinitionFormatter>();
            container.Verify();
        }

        public static AssemblyJson Format(string code)
        {
            var (dll, xml) = Utility.Compile(code);
            using (AsyncScopedLifestyle.BeginScope(container))
            using (GenerationScope.Create(platformTarget, assemblyCollection))
            using (AssemblyScope.Create(xml))
            {
                var attributeFormatter = container.GetInstance<AttributeFormatter>();
                var memberDefinitionFormatter = container.GetInstance<MemberDefinitionFormatter>();
                return new AssemblyJson
                {
                    Attributes = attributeFormatter.Attributes(dll, "assembly").ToList(),
                    Types = dll.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()).Select(x =>
                    {
                        var jsonWriter = new InMemoryStreamingJsonWriter();
                        memberDefinitionFormatter.MemberDefinition(x, jsonWriter.StreamingJsonWriter);
                        return StructureDeserializer.Instance.Deserialize<IEntity>(new JsonTextReader(new StringReader(jsonWriter.Result)));
                    }).ToList(),
                };
            }
        }

        private static readonly Container container;
        private static readonly PlatformTarget platformTarget = PlatformTarget.TryParse("net46");
        private static readonly AssemblyCollection assemblyCollection = new AssemblyCollection(new LoggerFactory(), null);

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

        public sealed class InMemoryStreamingJsonWriter
        {
            private readonly StringBuilder _stringBuilder;
            private readonly StringWriter _stringWriter;
            private readonly JsonTextWriter _jsonTextWriter;

            public InMemoryStreamingJsonWriter()
            {
                _stringBuilder = new StringBuilder();
                _stringWriter = new StringWriter(_stringBuilder);
                _jsonTextWriter = new JsonTextWriter(_stringWriter);
                StreamingJsonWriter = new StreamingJsonWriter(new JsonBlobWriter(_jsonTextWriter));
            }

            public StreamingJsonWriter StreamingJsonWriter { get; }
            public string Result
            {
                get
                {
                    StreamingJsonWriter.CommitAsync().GetAwaiter().GetResult();
                    _jsonTextWriter.Flush();
                    _stringWriter.Flush();
                    return _stringBuilder.ToString();
                }
            }

            private sealed class JsonBlobWriter : IJsonBlobWriter
            {
                public JsonBlobWriter(JsonWriter writer)
                {
                    JsonWriter = writer;
                }

                public Task CommitAsync() => Task.CompletedTask;

                public Uri Uri => throw new NotImplementedException();

                public JsonWriter JsonWriter { get; }
            }
        }
    }
}