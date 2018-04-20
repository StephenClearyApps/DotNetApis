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
        /// If the entity is a type entity, then this is qualified by its declaring type(s), and both <see cref="QualifiedName"/> and <see cref="FullyQualifiedName"/> are qualified by both its declaring type(s) and namespace.
        /// </summary>
        public string SimpleName { get; }

        /// <summary>
        /// The name of the entity, qualified by its declaring type(s).
        /// If the entity is a type entity, then this is also qualified by its namespace, and is equal to <see cref="FullyQualifiedName"/>.
        /// </summary>
        public string QualifiedName { get; }

        /// <summary>
        /// The name of the entity, qualified by both its declaring type(s) and namespace.
        /// </summary>
        public string FullyQualifiedName { get; }
    }
}
