namespace DotNetApis.Structure.Literals
{
    /// <summary>
    /// The literal value <c>null</c>.
    /// </summary>
    public sealed class NullLiteral : ILiteral
    {
        public LiteralKind Kind => LiteralKind.Null;

        public override string ToString() => "null";
    }
}
