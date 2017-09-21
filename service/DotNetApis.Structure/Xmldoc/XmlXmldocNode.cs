using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Xmldoc
{
    public sealed class XmlXmldocNode : IXmldocNode
    {
        public XmlXmldocNode(XmlXmldocNodeKind kind, object attributes, IEnumerable<IXmldocNode> children)
        {
            Kind = kind;
            Attributes = attributes;
            Children = children.Where(x => x != null).ToList();
        }

        /// <summary>
        /// The kind of node represented by this object.
        /// </summary>
        [JsonProperty("k")]
        public XmlXmldocNodeKind Kind { get; set; }

        /// <summary>
        /// The additional attributes for this node. May be <c>null</c>.
        /// </summary>
        [JsonProperty("a")]
        public object Attributes { get; set; }

        /// <summary>
        /// The children of this node. Never <c>null</c>.
        /// </summary>
        [JsonProperty("c")]
        public IReadOnlyList<IXmldocNode> Children { get; }

        public override string ToString() => string.Join("", Children);
    }
}
