using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure.GenericConstraints
{
    /// <summary>
    /// new() generic constraint.
    /// </summary>
    public sealed class NewGenericConstraint : IGenericConstraint
    {
        public GenericConstraintKind Kind => GenericConstraintKind.New;

        public override string ToString() => "new()";
    }
}
