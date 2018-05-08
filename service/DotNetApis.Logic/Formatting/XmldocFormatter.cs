using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using DotNetApis.Structure.Locations;
using DotNetApis.Structure.Xmldoc;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using static DotNetApis.Common.Enumerables;

namespace DotNetApis.Logic.Formatting
{
    /// <summary>
    /// Formats XML documentation.
    /// </summary>
    public sealed class XmldocFormatter
    {
        private readonly ILogger<XmldocFormatter> _logger;
        private readonly IReferenceXmldocTable _referenceXmldocTable;
        private readonly TypeLocator _typeLocator;
        private readonly GenerationScope.Accessor _generationScope;
        private readonly AssemblyScope.Accessor _assemblyScope;

        public XmldocFormatter(ILoggerFactory loggerFactory, IReferenceXmldocTable referenceXmldocTable, TypeLocator typeLocator, GenerationScope.Accessor generationScope, AssemblyScope.Accessor assemblyScope)
        {
            _logger = loggerFactory.CreateLogger<XmldocFormatter>();
            _referenceXmldocTable = referenceXmldocTable;
            _typeLocator = typeLocator;
            _generationScope = generationScope;
            _assemblyScope = assemblyScope;
        }

        /// <summary>
        /// Formats the XML documentation for a member.
        /// </summary>
        /// <param name="member">The member to describe.</param>
        public Xmldoc Xmldoc(IMemberDefinition member)
        {
            var xmldoc = _assemblyScope.Current.Xmldoc;
            if (xmldoc == null)
                return null;
            var memberXmldocId = member.MemberXmldocIdentifier();
            var doc = xmldoc.Descendants("member").FirstOrDefault(x => x.Attribute("name")?.Value == memberXmldocId);
            if (doc == null)
            {
                _logger.MemberNotFound(memberXmldocId);
                return null;
            }

            var result = new Xmldoc();

            // First, try to find <summary>, and fall back on <value> if not found.
            var basicDoc = doc.Element("summary") ?? doc.Element("value");
            if (basicDoc != null)
                result.Basic = XmldocNode(basicDoc);

            var remarksDoc = doc.Element("remarks");
            if (remarksDoc != null)
                result.Remarks = XmldocNode(remarksDoc);

            var examplesDoc = doc.Elements("example");
            if (examplesDoc.Any())
                result.Examples = examplesDoc.Select(XmldocNode).ToList();

            // <seealso> tags can be used either at the top-level or inline.
            var seealsoDoc = doc.Descendants("seealso");
            if (seealsoDoc.Any())
                result.SeeAlso = seealsoDoc.Select(XmldocNode).ToList();

            var exceptionsDoc = doc.Elements("exception");
            if (exceptionsDoc.Any())
                result.Exceptions = exceptionsDoc.Select(XmldocNode).ToList();

            var method = TryGetXmldocRepresentativeMethod(member);
            if (method != null)
            {
                var returnsDoc = doc.Element("returns");
                if (returnsDoc != null)
                    result.Returns = XmldocNode(returnsDoc);
            }

            return result;
        }

        /// <summary>
        /// Formats the XML documentation for a generic parameter of a member (<c>typeparam</c> documentation).
        /// </summary>
        /// <param name="member">The member with the generic parameter.</param>
        /// <param name="parameter">The generic parameter.</param>
        public IXmldocNode XmldocNodeForGenericParameter(IMemberDefinition member, GenericParameter parameter)
        {
            var xmldoc = _assemblyScope.Current.Xmldoc;
            if (xmldoc == null)
                return null;
            var memberXmldocId = member.MemberXmldocIdentifier();
            var doc = xmldoc.Descendants("member").FirstOrDefault(x => x.Attribute("name")?.Value == memberXmldocId);
            if (doc == null)
                return null;

            var parameterName = parameter.Name;
            var typeparamDoc = doc.Elements("typeparam").FirstOrDefault(y => y.Attribute("name")?.Value == parameterName);
            if (typeparamDoc == null)
            {
                _logger.TypeparamNotFound(parameterName, memberXmldocId);
                return null;
            }
            return XmldocNode(typeparamDoc);
        }

        /// <summary>
        /// Formats the XML documentation for a parameter of a member (<c>param</c> documentation).
        /// </summary>
        /// <param name="member">The member with the generic parameter.</param>
        /// <param name="parameter">The parameter.</param>
        public IXmldocNode XmldocNodeForParameter(IMemberDefinition member, ParameterDefinition parameter)
        {
            var xmldoc = _assemblyScope.Current.Xmldoc;
            if (xmldoc == null)
                return null;
            var memberXmldocId = member.MemberXmldocIdentifier();
            var doc = xmldoc.Descendants("member").FirstOrDefault(x => x.Attribute("name")?.Value == memberXmldocId);
            if (doc == null)
                return null;

            var parameterName = parameter.Name;
            var paramDoc = doc.Elements("param").FirstOrDefault(y => y.Attribute("name")?.Value == parameterName);
            if (paramDoc == null)
            {
                _logger.ParamNotFound(parameterName, memberXmldocId);
                return null;
            }
            return XmldocNode(paramDoc);
        }

        /// <summary>
        /// Gets the "representative method" for this member (if any), which holds the XML documentation for the return value and parameters. Returns <c>null</c> if there is no representative method.
        /// </summary>
        /// <param name="member">The member.</param>
        private static MethodDefinition TryGetXmldocRepresentativeMethod(IMemberDefinition member)
        {
            if (member is MethodDefinition method)
                return method;
            if (member is PropertyDefinition property)
                return property.GetMethod ?? property.SetMethod;
            if (member is TypeDefinition type && type.IsDelegate())
                return type.Methods.First(x => x.Name == "Invoke");
            return null;
        }

        /// <summary>
        /// Formats the xmldoc element to structured xmldoc. May return <c>null</c> if there is no structured representation for this xmldoc element.
        /// </summary>
        private IXmldocNode XmldocNode(XElement source)
        {
            // Top-level entries are output as divs.
            if (source.Name == "summary" || source.Name == "value" || source.Name == "remarks" || source.Name == "example" ||
                source.Name == "exception" || source.Name == "typeparam" || source.Name == "returns" || source.Name == "param")
                return XmldocNode(source, XmlXmldocNodeKind.Div);

            // <c> tags represent inline code.
            if (source.Name == "c")
                return XmldocNode(source, XmlXmldocNodeKind.InlineCode);

            // <code> tags represent block code.
            if (source.Name == "code")
                return XmldocNode(source, XmlXmldocNodeKind.BlockCode);

            // <filterpriority> is an undocumented extension used by VB intellisense; we ignore it.
            if (source.Name == "filterpriority")
                return null;

            // <para> is for paragraphs.
            if (source.Name == "para")
                return XmldocNode(source, XmlXmldocNodeKind.Div);

            // <paramref> refers to a named parameter.
            if (source.Name == "paramref")
                return XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(source.Attribute("name")?.Value));

            // <typeparamref> refers to a type parameter.
            if (source.Name == "typeparamref")
                return XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(source.Attribute("name")?.Value));

            // <permission> is something no one understands, and there are no occurrences of it in the wild, so we just ignore it.
            if (source.Name == "permission")
                return null;

            // <see> and <seealso> refer to another member.
            if (source.Name == "see" || source.Name == "seealso")
            {
                var hasContents = source.Nodes().Any();
                var langword = source.Attribute("langword")?.Value;
                if (langword != null)
                    return hasContents ? XmldocNode(source, XmlXmldocNodeKind.InlineCode) : XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(langword));

                var cref = source.Attribute("cref")?.Value;
                if (cref == null)
                {
                    _logger.MissingSeeTarget();
                    return null;
                }
                if (cref.StartsWith("!:") || cref.StartsWith("N:"))
                    return XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(cref.Substring(2)));
                var useQualifiedName = source.Attribute("qualifyHint")?.Value == "true";
                var useOverload = source.Attribute("autoUpgrade")?.Value == "true";
                var (structuredLocation, friendlyName) = TryResolveXmldocIdentifier(cref);
                var defaultText = (useQualifiedName ? friendlyName?.QualifiedName : friendlyName?.SimpleName) ?? cref;
                var attributes = new { l = structuredLocation, t = friendlyName?.FullyQualifiedName };
                return hasContents ?
                    XmldocNode(source, XmlXmldocNodeKind.See, attributes) :
                    XmldocNode(XmlXmldocNodeKind.See, attributes, XmldocNode(defaultText));
            }

            // <list> is half-baked support for lists and tables. It's defined to work for definition lists, too, but no one else supports that, so we don't either.
            if (source.Name == "list")
            {
                var type = source.Attribute("type")?.Value ?? "bullet";
                if (type != "table")
                    return XmldocNode(type == "number" ? XmlXmldocNodeKind.OrderedList : XmlXmldocNodeKind.UnorderedList, null,
                        source.Descendants("description").Select(x => XmldocNode(x, XmlXmldocNodeKind.ListItem)));
                var header = source.Element("listheader");
                var structuredHeader = header == null ? null :
                    XmldocNode(XmlXmldocNodeKind.TableRow, null, header.Descendants("term").Select(x => XmldocNode(x, XmlXmldocNodeKind.TableHeaderData)));
                return XmldocNode(XmlXmldocNodeKind.Table, null, Return(structuredHeader).Concat(
                    source.Elements("item").Select(item => XmldocNode(XmlXmldocNodeKind.TableRow, null,
                        item.Descendants("description").Select(x => XmldocNode(x, XmlXmldocNodeKind.TableData))))
                    ));
            }

            // Supported HTML tags.

            if (source.Name == "div" || source.Name == "p")
                return XmldocNode(source, XmlXmldocNodeKind.Div);
            if (source.Name == "i")
                return XmldocNode(source, XmlXmldocNodeKind.Italic);
            if (source.Name == "b")
                return XmldocNode(source, XmlXmldocNodeKind.Bold);
            if (source.Name == "a" && source.Attribute("href")?.Value != null)
                return XmldocNode(source, XmlXmldocNodeKind.Link, new { h = source.Attribute("href")?.Value });
            if (source.Name == "table")
                return XmldocNode(source, XmlXmldocNodeKind.Table);
            if (source.Name == "tr")
                return XmldocNode(source, XmlXmldocNodeKind.TableRow);
            if (source.Name == "th")
                return XmldocNode(source, XmlXmldocNodeKind.TableHeaderData);
            if (source.Name == "td")
                return XmldocNode(source, XmlXmldocNodeKind.TableData);
            if (source.Name == "ol")
                return XmldocNode(source, XmlXmldocNodeKind.OrderedList);
            if (source.Name == "ul")
                return XmldocNode(source, XmlXmldocNodeKind.UnorderedList);
            if (source.Name == "li")
                return XmldocNode(source, XmlXmldocNodeKind.ListItem);

            // All other elements are assumed to be HTML, and are treated as a <span>, extracting text nodes and transforming any child nodes.
            _logger.UnrecognizedTag(source.Name.ToString());
            return XmldocNode(source, XmlXmldocNodeKind.Span);
        }

        /// <summary>
        /// Formats the xmldoc element to a structured xmldoc object of the specified kind with the specified attributes. The xmldoc element's children are processed as the contents of the structured xmldoc object.
        /// </summary>
        private IXmldocNode XmldocNode(XElement element, XmlXmldocNodeKind kind, object attributes = null) => new XmlXmldocNode(kind, attributes, element.Nodes().Select(XmldocNode));

        /// <summary>
        /// Formats a structured xmldoc object of the specified kind with the specified attributes and child structured xmldoc objects.
        /// </summary>
        private static IXmldocNode XmldocNode(XmlXmldocNodeKind kind, object attributes, params IXmldocNode[] children) => XmldocNode(kind, attributes, children.AsEnumerable());

        /// <summary>
        /// Formats a structured xmldoc object of the specified kind with the specified attributes and child structured xmldoc objects.
        /// </summary>
        private static IXmldocNode XmldocNode(XmlXmldocNodeKind kind, object attributes, IEnumerable<IXmldocNode> children) => new XmlXmldocNode(kind, attributes, children);

        private static IXmldocNode XmldocNode(string text)
        {
            if (text == null)
                return null;
            return new StringXmldocNode
            {
                Text = text,
            };
        }

        /// <summary>
        /// Formats the xmldoc node to structured xmldoc. May return <c>null</c> if there is no structured representation for this xmldoc node.
        /// </summary>
        private IXmldocNode XmldocNode(XNode source)
        {
            if (source is XElement element)
                return XmldocNode(element);
            return XmldocNode(source.ToString(SaveOptions.DisableFormatting).MinimizeWhitespace());
        }

        private (ILocation Location, FriendlyName FriendlyName) TryResolveXmldocIdentifier(string xmldocid)
        {
            var result = _typeLocator.TryGetLocationAndFriendlyNameFromXmldocId(xmldocid);
            if (result != null)
                return result.Value;

            // Not found in any loaded dll. This is likely an xmldoc error, but we'll (quickly) check across all platform assembly references for our target just in case.
            var record = _referenceXmldocTable.TryGetRecord(_generationScope.Current.PlatformTarget, xmldocid);
            if (record != null)
                return (new ReferenceLocation { DnaId = record.Value.DnaId }, record.Value.FriendlyName);

            return (null, null);
        }
    }

	internal static partial class Logging
	{
		public static void MemberNotFound(this ILogger<XmldocFormatter> logger, string xmldocid) =>
			Logger.Log(logger, 1, LogLevel.Debug, "Unable to find xmldoc <member> tag with attribute @name matching {xmldocid}", xmldocid, null);

		public static void TypeparamNotFound(this ILogger<XmldocFormatter> logger, string name, string xmldocid) =>
			Logger.Log(logger, 2, LogLevel.Warning, "Unable to find xmldoc <typeparam> tag with attribute @name matching {name} for member {xmldocid}", name, xmldocid, null);

		public static void ParamNotFound(this ILogger<XmldocFormatter> logger, string name, string xmldocid) =>
			Logger.Log(logger, 3, LogLevel.Warning, "Unable to find xmldoc <param> tag with attribute @name matching {name} for member {xmldocid}", name, xmldocid, null);

		public static void MissingSeeTarget(this ILogger<XmldocFormatter> logger) =>
			Logger.Log(logger, 4, LogLevel.Warning, "Xmldoc error: <see> or <seealso> element does not have langword or cref attributes", null);

		public static void UnrecognizedTag(this ILogger<XmldocFormatter> logger, string tag) =>
			Logger.Log(logger, 5, LogLevel.Warning, "Unrecognized xmldoc tag {tag}", tag, null);
	}
}
