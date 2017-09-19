using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    public sealed class StructuredXmldocNode
    {
        public StructuredXmldocNode(XmldocEntityKind kind, object attributes, IEnumerable<StructuredXmldocNode> children)
        {
            Kind = kind;
            Attributes = attributes;
            Children = children.Where(x => x != null).ToList();
        }

        /// <summary>
        /// The kind of node represented by this object.
        /// </summary>
        [JsonProperty("k")]
        public XmldocEntityKind Kind { get; set; }

        /// <summary>
        /// The additional attributes for this node. May be <c>null</c>.
        /// </summary>
        [JsonProperty("a")]
        public object Attributes { get; set; }

        /// <summary>
        /// The children of this node. Never <c>null</c>.
        /// </summary>
        [JsonProperty("c")]
        public IReadOnlyList<StructuredXmldocNode> Children { get; }
    }
}
