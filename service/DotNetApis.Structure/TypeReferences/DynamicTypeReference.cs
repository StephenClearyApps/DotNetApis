namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// A dynamic reference; that is, an <c>object</c> that is treated by the compiler as <c>dynamic</c>.
    /// </summary>
    public sealed class DynamicTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.Dynamic;

        public override string ToString() => "dynamic";
    }
}
