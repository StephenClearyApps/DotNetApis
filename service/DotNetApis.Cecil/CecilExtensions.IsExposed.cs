using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public static partial class CecilExtensions
    {
        /// <summary>
        /// Whether this type is accessible from another assembly (if this is a nested type, then assuming its declaring type is accessible).
        /// </summary>
        public static bool IsExposed(this TypeDefinition type) => type.IsPublic || type.IsNestedPublic || type.IsNestedFamily || type.IsNestedFamilyOrAssembly;

        /// <summary>
        /// Whether this field is accessible from another assembly (assuming its declaring type is accessible).
        /// </summary>
        public static bool IsExposed(this FieldDefinition field)
        {
            if (field.IsPublic)
                return true;

            // Protected fields are only accessible if the declaring type is not sealed.
            if (field.IsFamily || field.IsFamilyOrAssembly)
                return !field.DeclaringType.IsSealed;

            return false;
        }

        /// <summary>
        /// Whether this method is accessible from another assembly (assuming its declaring type is accessible).
        /// </summary>
        public static bool IsExposed(this MethodDefinition method)
        {
            if (method.IsPublic)
                return true;

            // Protected methods are only accessible if the declaring type is not sealed.
            if (method.IsFamily || method.IsFamilyOrAssembly)
                return !method.DeclaringType.IsSealed;

            // Explicitly implemented interface methods are only accessible if their interface type is accessible (in addition to its declaring types).
            var interfaceMethod = method.GetExplicitlyImplementedInterfaceMethod();
            if (interfaceMethod != null)
                return interfaceMethod.DeclaringType.DeclaringTypesAndThis().All(x => x.Resolve().IsExposed());

            return false;
        }

        /// <summary>
        /// Whether this property is accessible from another assembly (assuming its declaring type is accessible).
        /// </summary>
        public static bool IsExposed(this PropertyDefinition property) =>
            (property.GetMethod != null && property.GetMethod.IsExposed()) || (property.SetMethod != null && property.SetMethod.IsExposed());

        /// <summary>
        /// Whether this event is accessible from another assembly (assuming its declaring type is accessible).
        /// </summary>
        public static bool IsExposed(this EventDefinition @event) => @event.AddMethod.IsExposed();

        public static IEnumerable<IMemberDefinition> ExposedMembers(this TypeDefinition type)
        {
            var properties = type.Properties.Where(x => x.IsExposed()).ToList();
            var events = type.Events.Where(x => x.IsExposed()).ToList();
            var methods = type.Methods.Where(x => x.IsExposed() &&
                !properties.Any(y => x == y.GetMethod || x == y.SetMethod) &&
                !events.Any(y => x == y.AddMethod || x == y.RemoveMethod));
            var fields = type.Fields.Where(x => x.IsExposed());
            var nestedTypes = type.NestedTypes.Where(x => x.IsExposed());
            return Enumerable.Empty<IMemberDefinition>().Concat(properties).Concat(events).Concat(methods).Concat(fields).Concat(nestedTypes);
        }
    }
}
