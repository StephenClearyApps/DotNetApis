using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// This must be kept in sync with util/entities/xmldoc.ts
    /// </summary>
    public enum XmldocEntityKind
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
        // TODO: others

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
