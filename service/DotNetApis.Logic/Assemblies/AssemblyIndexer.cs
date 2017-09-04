using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// A type used to update two different collections about members within an assembly.
    /// </summary>
    public sealed class AssemblyIndexer
    {
        private readonly IDictionary<string, FriendlyName> _dnaidToFriendlyName;
        private readonly IDictionary<string, string> _xmldocIdToDnaId;

        public AssemblyIndexer(IDictionary<string, FriendlyName> dnaidToFriendlyName, IDictionary<string, string> xmldocIdToDnaId)
        {
            _dnaidToFriendlyName = dnaidToFriendlyName;
            _xmldocIdToDnaId = xmldocIdToDnaId;
        }

        /// <summary>
        /// Adds a member to both collections.
        /// </summary>
        /// <param name="member">The member to add.</param>
        public void Add(IMemberDefinition member)
        {
            var dnaId = member.MemberDnaId();
            _dnaidToFriendlyName[dnaId] = FriendlyName.Create(member);
            _xmldocIdToDnaId[member.MemberXmldocIdentifier()] = dnaId;
        }

        /// <summary>
        /// Adds a method overload group to both collections.
        /// </summary>
        /// <param name="method">The method to add.</param>
        public void AddOverload(MethodDefinition method)
        {
            var dnaId = method.OverloadDnaId();
            _dnaidToFriendlyName[dnaId] = FriendlyName.CreateOverload(method);
            _xmldocIdToDnaId[method.OverloadXmldocIdentifier()] = dnaId;
        }

        /// <summary>
        /// Adds the given type and all of its exposed members to both collections.
        /// </summary>
        /// <param name="type">The type to add.</param>
        public void AddExposed(TypeDefinition type)
        {
            Add(type);
            foreach (var member in type.ExposedMembers())
            {
                if (member is TypeDefinition nestedType)
                {
                    AddExposed(nestedType);
                }
                else
                {
                    Add(member);
                    if (member is MethodDefinition method)
                        AddOverload(method);
                }
            }
        }

        /// <summary>
        /// Adds all exposed types in all modules of a given assembly to both collections.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void AddExposed(AssemblyDefinition assembly)
        {
            foreach (var type in assembly.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()))
                AddExposed(type);
        }
    }
}
