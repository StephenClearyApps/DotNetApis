using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure.GenericConstraints
{
    /// <summary>
    /// Reference type generic constraint.
    /// </summary>
    public sealed class ClassGenericConstraint : IGenericConstraint
    {
        public GenericConstraintKind Kind => GenericConstraintKind.Class;

        public override string ToString() => "class";
    }
}
