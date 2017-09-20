using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Structure.Entities;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    public sealed class ModifiersFormatter
    {
        /// <summary>
        /// Formats OOP modifiers (static, abstract, sealed) for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        public EntityModifiers TypeDefinitionModifiers(TypeDefinition type)
        {
            if (type.IsAbstract && type.IsSealed)
                return EntityModifiers.Static;
            if (type.IsAbstract)
                return EntityModifiers.Abstract;
            if (type.IsSealed)
                return EntityModifiers.Sealed;
            return EntityModifiers.None;
        }

        /// <summary>
        /// Formats OOP modifiers (static, sealed, override, abstract, and/or virtual) for a method.
        /// </summary>
        /// <param name="method">The method.</param>
        public EntityModifiers MethodModifiers(MethodDefinition method)
        {
            if (method.IsStatic)
                return EntityModifiers.Static;
            var isOverride = method.IsOverride();
            if (isOverride)
            {
                if (method.IsFinal)
                    return EntityModifiers.Override | EntityModifiers.Sealed;
                return EntityModifiers.Override;
            }

            if (method.IsAbstract)
                return EntityModifiers.Abstract;
            if (method.IsFinal)
                return EntityModifiers.None;
            if (method.IsVirtual)
                return EntityModifiers.Virtual;
            return EntityModifiers.None; // Constructors do end up here.
        }
    }
}
