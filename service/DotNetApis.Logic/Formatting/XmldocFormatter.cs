using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Common;
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
            var doc = xmldoc.Descendants(XNames.Member).FirstOrDefault(x => x.Attribute(XNames.Name)?.Value == memberXmldocId);
            if (doc == null)
            {
                _logger.MemberNotFound(memberXmldocId);
                return null;
            }

            var result = new Xmldoc();

            // First, try to find <summary>, and fall back on <value> if not found.
            var basicDoc = doc.Element(XNames.Summary) ?? doc.Element(XNames.Value);
            if (basicDoc != null)
                result.Basic = XmldocNode(basicDoc);

            var remarksDoc = doc.Element(XNames.Remarks);
            if (remarksDoc != null)
                result.Remarks = XmldocNode(remarksDoc);

            var examplesDoc = doc.Elements(XNames.Example);
            if (examplesDoc.Any())
                result.Examples = examplesDoc.Select(XmldocNode).ToList();

            // <seealso> tags can be used either at the top-level or inline.
            var seealsoDoc = doc.Descendants(XNames.Seealso);
            if (seealsoDoc.Any())
                result.SeeAlso = seealsoDoc.Select(XmldocNode).ToList();

            var exceptionsDoc = doc.Elements(XNames.Exception);
            if (exceptionsDoc.Any())
                result.Exceptions = exceptionsDoc.Select(XmldocNode).ToList();

            var method = TryGetXmldocRepresentativeMethod(member);
            if (method != null)
            {
                var returnsDoc = doc.Element(XNames.Returns);
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
            var doc = xmldoc.Descendants(XNames.Member).FirstOrDefault(x => x.Attribute(XNames.Name)?.Value == memberXmldocId);
            if (doc == null)
                return null;

            var parameterName = parameter.Name;
            var typeparamDoc = doc.Elements(XNames.Typeparam).FirstOrDefault(y => y.Attribute(XNames.Name)?.Value == parameterName);
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
            var doc = xmldoc.Descendants(XNames.Member).FirstOrDefault(x => x.Attribute(XNames.Name)?.Value == memberXmldocId);
            if (doc == null)
                return null;

            var parameterName = parameter.Name;
            var paramDoc = doc.Elements(XNames.Param).FirstOrDefault(y => y.Attribute(XNames.Name)?.Value == parameterName);
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
            if (source.Name == XNames.Summary || source.Name == XNames.Value || source.Name == XNames.Remarks || source.Name == XNames.Example ||
                source.Name == XNames.Exception || source.Name == XNames.Typeparam || source.Name == XNames.Returns || source.Name == XNames.Param)
                return XmldocNode(source, XmlXmldocNodeKind.Div);

            // <c> tags represent inline code.
            if (source.Name == XNames.C)
                return XmldocNode(source, XmlXmldocNodeKind.InlineCode);

            // <code> tags represent block code.
            if (source.Name == XNames.Code)
                return XmldocNode(source, XmlXmldocNodeKind.BlockCode);

            // <filterpriority> is an undocumented extension used by VB intellisense; we ignore it.
            if (source.Name == XNames.Filterpriority)
                return null;

            // <para> is for paragraphs.
            if (source.Name == XNames.Para)
                return XmldocNode(source, XmlXmldocNodeKind.Div);

            // <paramref> refers to a named parameter.
            if (source.Name == XNames.Paramref)
                return XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(source.Attribute(XNames.Name)?.Value));

            // <typeparamref> refers to a type parameter.
            if (source.Name == XNames.Typeparamref)
                return XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(source.Attribute(XNames.Name)?.Value));

            // <permission> is something no one understands, and there are no occurrences of it in the wild, so we just ignore it.
            if (source.Name == XNames.Permission)
                return null;

            // <see> and <seealso> refer to another member.
            if (source.Name == XNames.See || source.Name == XNames.Seealso)
            {
                var hasContents = source.Nodes().Any();
                var langword = source.Attribute(XNames.Langword)?.Value;
                if (langword != null)
                    return hasContents ? XmldocNode(source, XmlXmldocNodeKind.InlineCode) : XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(langword));

                var cref = source.Attribute(XNames.Cref)?.Value;
                if (cref == null)
                {
                    _logger.MissingSeeTarget();
                    return null;
                }
                if (cref.StartsWith("!:") || cref.StartsWith("N:"))
                    return XmldocNode(XmlXmldocNodeKind.InlineCode, null, XmldocNode(cref.Substring(2)));
                var useQualifiedName = source.Attribute(XNames.QualifyHint)?.Value == "true";
                var useOverload = source.Attribute(XNames.AutoUpgrade)?.Value == "true";
                var (structuredLocation, friendlyName) = TryResolveXmldocIdentifier(cref);
                var defaultText = (useQualifiedName ? friendlyName?.QualifiedName : friendlyName?.SimpleName) ?? cref;
                var attributes = new { l = structuredLocation, t = friendlyName?.FullyQualifiedName };
                return hasContents ?
                    XmldocNode(source, XmlXmldocNodeKind.See, attributes) :
                    XmldocNode(XmlXmldocNodeKind.See, attributes, XmldocNode(defaultText));
            }

            // <list> is half-baked support for lists and tables. It's defined to work for definition lists, too, but no one else supports that, so we don't either.
            if (source.Name == XNames.List)
            {
                var type = source.Attribute(XNames.Type)?.Value ?? "bullet";
                if (type != "table")
                    return XmldocNode(type == "number" ? XmlXmldocNodeKind.OrderedList : XmlXmldocNodeKind.UnorderedList, null,
                        source.Descendants(XNames.Description).Select(x => XmldocNode(x, XmlXmldocNodeKind.ListItem)));
                var header = source.Element(XNames.Listheader);
                var structuredHeader = header == null ? null :
                    XmldocNode(XmlXmldocNodeKind.TableRow, null, header.Descendants(XNames.Term).Select(x => XmldocNode(x, XmlXmldocNodeKind.TableHeaderData)));
                return XmldocNode(XmlXmldocNodeKind.Table, null, Return(structuredHeader).Concat(
                    source.Elements(XNames.Item).Select(item => XmldocNode(XmlXmldocNodeKind.TableRow, null,
                        item.Descendants(XNames.Description).Select(x => XmldocNode(x, XmlXmldocNodeKind.TableData))))
                    ));
            }

            // Supported HTML tags.

            if (source.Name == XNames.Div || source.Name == XNames.P)
                return XmldocNode(source, XmlXmldocNodeKind.Div);
            if (source.Name == XNames.I)
                return XmldocNode(source, XmlXmldocNodeKind.Italic);
            if (source.Name == XNames.B)
                return XmldocNode(source, XmlXmldocNodeKind.Bold);
            if (source.Name == XNames.A && source.Attribute(XNames.Href)?.Value != null)
                return XmldocNode(source, XmlXmldocNodeKind.Link, new { h = source.Attribute(XNames.Href)?.Value });
            if (source.Name == XNames.Table)
                return XmldocNode(source, XmlXmldocNodeKind.Table);
            if (source.Name == XNames.Tr)
                return XmldocNode(source, XmlXmldocNodeKind.TableRow);
            if (source.Name == XNames.Th)
                return XmldocNode(source, XmlXmldocNodeKind.TableHeaderData);
            if (source.Name == XNames.Td)
                return XmldocNode(source, XmlXmldocNodeKind.TableData);
            if (source.Name == XNames.Ol)
                return XmldocNode(source, XmlXmldocNodeKind.OrderedList);
            if (source.Name == XNames.Ul)
                return XmldocNode(source, XmlXmldocNodeKind.UnorderedList);
            if (source.Name == XNames.Li)
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

	    private static class XNames
	    {
		    public static readonly XName A = "a";
		    public static readonly XName AutoUpgrade = "autoUpgrade";
		    public static readonly XName B = "b";
		    public static readonly XName C = "c";
		    public static readonly XName Code = "code";
		    public static readonly XName Cref = "cref";
		    public static readonly XName Description = "description";
		    public static readonly XName Div = "div";
		    public static readonly XName Example = "example";
		    public static readonly XName Exception = "exception";
		    public static readonly XName Filterpriority = "filterpriority";
		    public static readonly XName Href = "href";
		    public static readonly XName I = "i";
		    public static readonly XName Item = "item";
		    public static readonly XName Langword = "langword";
		    public static readonly XName Li = "li";
		    public static readonly XName List = "list";
		    public static readonly XName Listheader = "listheader";
		    public static readonly XName Member = "member";
		    public static readonly XName Name = "name";
		    public static readonly XName Ol = "ol";
		    public static readonly XName P = "p";
		    public static readonly XName Para = "para";
		    public static readonly XName Param = "param";
		    public static readonly XName Paramref = "paramref";
		    public static readonly XName Permission = "permission";
		    public static readonly XName QualifyHint = "qualifyHint";
		    public static readonly XName Remarks = "remarks";
		    public static readonly XName Returns = "returns";
		    public static readonly XName See = "see";
		    public static readonly XName Seealso = "seealso";
		    public static readonly XName Summary = "summary";
		    public static readonly XName Table = "table";
		    public static readonly XName Td = "td";
		    public static readonly XName Term = "term";
		    public static readonly XName Th = "th";
		    public static readonly XName Tr = "tr";
		    public static readonly XName Type = "type";
			public static readonly XName Typeparam = "typeparam";
		    public static readonly XName Typeparamref = "typeparamref";
		    public static readonly XName Ul = "ul";
			public static readonly XName Value = "value";
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
