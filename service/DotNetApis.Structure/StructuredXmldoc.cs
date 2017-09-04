using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.StructuredFormatter
{
    public sealed class StructuredXmldoc
    {
        public StructuredXmldoc(XmldocEntityKind kind, object attributes, IEnumerable<StructuredXmldoc> children)
        {
            Kind = kind;
            Attributes = attributes;
            Children = children.Where(x => x != null).ToList();
        }

        /// <summary>
        /// The kind of node represented by this object.
        /// </summary>
        [JsonIgnore]
        public XmldocEntityKind Kind { get; set; }

        [JsonProperty("k")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int KindJson
        {
            get => (int) Kind;
            set => Kind = (XmldocEntityKind) value;
        }

        /// <summary>
        /// The additional attributes for this node. May be <c>null</c>.
        /// </summary>
        [JsonProperty("a")]
        public object Attributes { get; set; }

        /// <summary>
        /// The children of this node. Never <c>null</c>.
        /// </summary>
        [JsonProperty("c")]
        public IReadOnlyList<StructuredXmldoc> Children { get; }
    }
}
