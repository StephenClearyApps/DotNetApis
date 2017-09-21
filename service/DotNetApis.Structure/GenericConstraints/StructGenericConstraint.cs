using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure.GenericConstraints
{
    /// <summary>
    /// Value type generic constraint.
    /// </summary>
    public sealed class StructGenericConstraint : IGenericConstraint
    {
        public GenericConstraintKind Kind => GenericConstraintKind.Struct;

        public override string ToString() => "struct";
    }
}
