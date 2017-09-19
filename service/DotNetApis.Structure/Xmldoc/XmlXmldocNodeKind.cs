using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Xmldoc
{
    /// <summary>
    /// This must be kept in sync with util/entities/xmldoc.ts
    /// </summary>
    [JsonConverter(typeof(IntEnumConverter))]
    public enum XmlXmldocNodeKind
    {
        // Common Xmldoc entities.
        Div = 0,
        InlineCode = 1,
        BlockCode = 2,
        See = 3, // l: location

        // "Clean" (whitelisted) HTML tags.
        Span,
        Bold,
        Italic,
        Link, // h: href

        // Uncommon Xmldoc entities.
        UnorderedList,
        OrderedList,
        ListItem,
        Table,
        TableRow,
        TableData,
        TableHeaderData,
    }
}
