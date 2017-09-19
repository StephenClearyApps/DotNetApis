using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure.Entities
{
    public sealed class ObjectMembers
    {
        public IReadOnlyList<IEntity> Lifetime { get; set; }

        public IReadOnlyList<IEntity> Static { get; set; }

        public IReadOnlyList<IEntity> Instance { get; set; }

        //public IReadOnlyList<IEntity> 

        public IReadOnlyList<IEntity> NestedTypes { get; set; }
    }
}
