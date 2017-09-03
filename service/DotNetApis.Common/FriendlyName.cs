namespace DotNetApis.Common
{
    /// <summary>
    /// A friendly name for an entity. Friendly names are human-readable, e.g., "Task&lt;TResult&gt;".
    /// </summary>
    public sealed class FriendlyName
    {
        public FriendlyName(string simpleName, string qualifiedName, string fullyQualifiedName)
        {
            SimpleName = simpleName;
            QualifiedName = qualifiedName;
            FullyQualifiedName = fullyQualifiedName;
        }
        
        /// <summary>
        /// The simple name of the entity, including generic parameters.
        /// </summary>
        public string SimpleName { get; }

        /// <summary>
        /// The name of the entity, qualified by its declaring type(s) or namespace.
        /// </summary>
        public string QualifiedName { get; }

        /// <summary>
        /// The name of the entity, qualified by its declaring type(s) and namespace.
        /// </summary>
        public string FullyQualifiedName { get; }
    }
}
