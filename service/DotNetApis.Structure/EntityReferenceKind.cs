namespace DotNetApis.Structure
{
    /// <summary>
    /// This must be kept in sync with constants/entityReferenceKinds.js
    /// </summary>
    public enum EntityReferenceKind
    {
        Type = 0,
        Keyword,
        GenericInstance,
        Dynamic,
        GenericParameter,
        Array,
        Reqmod,
        Pointer,
    }
}
