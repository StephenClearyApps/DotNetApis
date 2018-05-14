using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetApis.Logic
{
    public sealed class XmldocIndex
    {
	    private readonly Dictionary<string, XElement> _memberXmldocs = new Dictionary<string, XElement>();

	    public XmldocIndex(XContainer xmldoc)
	    {
		    if (xmldoc == null)
			    return;
		    foreach (var element in xmldoc.Element(XNames.Doc)?.Element(XNames.Members)?.Elements(XNames.Member) ?? Enumerable.Empty<XElement>())
			    _memberXmldocs[element.Attribute(XNames.Name)?.Value ?? ""] = element;
		}

	    public bool Empty => _memberXmldocs.Count == 0;

	    public XElement TryLookup(string memberName) => _memberXmldocs.TryGetValue(memberName, out var value) ? value : null;

		private static class XNames
		{
			public static readonly XName Doc = "doc";
			public static readonly XName Member = "member";
			public static readonly XName Members = "members";
			public static readonly XName Name = "name";
		}
	}
}
