using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Nito.Comparers;

namespace DotNetApis.Logic
{
    /// <summary>
    /// Defines semantic ordering rules for type members.
    /// </summary>
    public static class SemanticOrdering
    {
        /// <summary>
        /// Whether a method is an Asynchronous Programming Model "Begin*" method.
        /// </summary>
        /// <param name="member">The method to check.</param>
        private static bool IsApmBeginMethod(IMemberDefinition member)
        {
            if (!(member is MethodDefinition method))
                return false;
            if (!method.Name.StartsWith("Begin"))
                return false;
            if (method.ReturnType.FullName != "System.IAsyncResult")
                return false;
            return member.DeclaringType.Methods.Any(x => x.Name == "End" + method.Name.Substring(5));
        }

        /// <summary>
        /// Whether a method is an Asynchronous Programming Model "End*" method.
        /// </summary>
        /// <param name="member">The method to check.</param>
        private static bool IsApmEndMethod(IMemberDefinition member)
        {
            if (!(member is MethodDefinition method))
                return false;
            if (!method.Name.StartsWith("End"))
                return false;
            if (method.Parameters.Count != 1)
                return false;
            if (method.Parameters[0].ParameterType.FullName != "System.IAsyncResult")
                return false;
            return member.DeclaringType.Methods.Any(x => x.Name == "Begin" + method.Name.Substring(3));
        }

        /// <summary>
        /// Whether a method is an Event-based Asynchronous Pattern "*Async" method.
        /// </summary>
        /// <param name="member">The method to check.</param>
        private static bool IsEapAsyncMethod(IMemberDefinition member)
        {
            if (!(member is MethodDefinition method))
                return false;
            if (!method.Name.EndsWith("Async"))
                return false;
            if (method.ReturnType.FullName != "System.Void")
                return false;
            return member.DeclaringType.Events.Any(x => x.Name == method.Name.Substring(0, method.Name.Length - 5) + "Completed");
        }

        /// <summary>
        /// Whether a method is an Event-based Asynchronous Pattern "*AsyncCancel" method.
        /// </summary>
        /// <param name="member">The method to check.</param>
        private static bool IsEapAsyncCancelMethod(IMemberDefinition member)
        {
            if (!(member is MethodDefinition method))
                return false;
            if (!method.Name.EndsWith("AsyncCancel"))
                return false;
            if (method.ReturnType.FullName != "System.Void")
                return false;
            return member.DeclaringType.Methods.Any(x => x.Name == method.Name.Substring(0, method.Name.Length - 6)) &&
                member.DeclaringType.Events.Any(x => x.Name == method.Name.Substring(0, method.Name.Length - 11) + "Completed");
        }

        /// <summary>
        /// Whether an event is an Event-based Asynchronous Pattern "*Completed" event.
        /// </summary>
        /// <param name="member">The event to check.</param>
        private static bool IsEapCompletedEvent(IMemberDefinition member)
        {
            if (!(member is EventDefinition @event))
                return false;
            if (!@event.Name.EndsWith("Completed"))
                return false;
            return member.DeclaringType.Methods.Any(x => x.Name == @event.Name.Substring(0, @event.Name.Length - 9));
        }

        /// <summary>
        /// Whether a method is a Task-based Asynchronous Pattern "*Async" method.
        /// </summary>
        /// <param name="member">The method to check.</param>
        private static bool IsTapMethod(IMemberDefinition member)
        {
            if (!(member is MethodDefinition method))
                return false;
            if (!method.Name.EndsWith("Async"))
                return false;
            return method.ReturnType.FullName == "System.Threading.Tasks.Task" || method.ReturnType.FullName == "System.Threading.Tasks.Task`1";
        }

        /// <summary>
        /// Strips common patterns from a member name to determine similarity with other methods. E.g., "DoAsync" becomes "Do", and "TryParse" becomes "Parse".
        /// </summary>
        /// <param name="member">The member.</param>
        private static string StripPatternsFromMemberName(IMemberDefinition member)
        {
            var name = member.Name;

            if (IsTapMethod(member))
                name = name.Substring(0, name.Length - 5);
            else if (IsApmBeginMethod(member))
                name = name.Substring(5);
            else if (IsApmEndMethod(member))
                name = name.Substring(3);
            else if (IsEapAsyncMethod(member))
                name = name.Substring(0, name.Length - 5);
            else if (IsEapAsyncCancelMethod(member))
                name = name.Substring(0, name.Length - 11);
            else if (IsEapCompletedEvent(member))
                name = name.Substring(0, name.Length - 9);

            if (name.StartsWith("Try"))
                name = name.Substring(3);

            return name;
        }

        /// <summary>
        /// Returns a value for semantic member ordering. 0-9 for lifetime management, 10-19 for static member, 20-29 for instance members, and 100 for nested types.
        /// </summary>
        public static int PrimaryMemberGrouping(IMemberDefinition member)
        {
            var method = member as MethodDefinition;
            var @event = member as EventDefinition;
            var property = member as PropertyDefinition;
            var field = member as FieldDefinition;

            // Object lifetime management

            // TODO: T/Task<T> static properties/values
            // TODO: order protected members much later than public.

            // Constructors
            if (method != null && method.IsConstructor)
                return 0;
            // Factory methods and async factory methods
            if (method != null && method.IsStatic && (method.ReturnType == method.DeclaringType ||
                (method.ReturnType.FullName == "System.Threading.Tasks.Task`1" && ((GenericInstanceType)method.ReturnType).GenericArguments.FirstOrDefault() == method.DeclaringType)))
                return 1;
            // Dispose methods
            if (method != null && method.Name == "Dispose")
                return 2;
            // Finalizers
            if (method != null && method.Name == "Finalize")
                return 3;

            // Static members

            // Static properties
            if (property != null && (property.GetMethod ?? property.SetMethod).IsStatic)
                return 10;
            // Static methods
            if (method != null && method.IsStatic)
                return 11;
            // Static events
            if (@event != null && @event.AddMethod.IsStatic)
                return 12;
            // Static fields
            if (field != null && field.IsStatic)
                return 13;

            // Instance members

            // Properties
            if (property != null)
                return 20;
            // Methods and completed events
            if (method != null || (@event != null && @event.Name.EndsWith("Completed")))
                return 21;
            // Other events
            if (@event != null)
                return 22;
            // Fields
            if (field != null)
                return 23;

            // Nested types

            return 100;
        }

        /// <summary>
        /// The logical number of parameters for a method or indexer property.
        /// </summary>
        /// <param name="member">The member to inspect.</param>
        private static int MemberParameterCount(IMemberDefinition member)
        {
            if (member is PropertyDefinition property)
            {
                if (property.GetMethod != null)
                    return property.GetMethod.Parameters.Count;
                return property.SetMethod.Parameters.Count - 1;
            }

            if (!(member is MethodDefinition method))
                return 0;

            var result = method.Parameters.Count;
            if (IsApmBeginMethod(member))
                result -= 2;
            else if (IsApmEndMethod(member))
                result = member.DeclaringType.Methods.Where(x => x.Name == "Begin" + member.Name.Substring(3)).Max(x => x.Parameters.Count - 2);
            return result;
        }

        /// <summary>
        /// Disambiguates the grouping established by <see cref="StripPatternsFromMemberName"/>. Example ordering: "Parse", "ParseAsync" (TAP), "BeginParse" (APM), "EndParse" (APM), "ParseAsync" (EAP), "ParseAsyncCancel" (EAP), "ParseCompleted" (EAP), "TryParse"
        /// </summary>
        /// <param name="member">The member to order.</param>
        private static int MemberNameOrdering(IMemberDefinition member)
        {
            if (IsTapMethod(member))
                return 1;
            if (IsApmBeginMethod(member))
                return 2;
            if (IsApmEndMethod(member))
                return 3;
            if (IsEapAsyncMethod(member))
                return 4;
            if (IsEapAsyncCancelMethod(member))
                return 5;
            if (IsEapCompletedEvent(member))
                return 6;
            if (member.Name.StartsWith("Try"))
                return 7;
            return 0;
        }

        public static readonly IFullComparer<IMemberDefinition> MemberComparer = ComparerBuilder.For<IMemberDefinition>()
            // Do a primary grouping (mostly by type)
            .OrderBy(PrimaryMemberGrouping)
            // Order/group by (modified) name
            .ThenBy(StripPatternsFromMemberName)
            // Order by (modified) number of parameters
            .ThenBy(MemberParameterCount)
            // TODO: order by parameter ordering in other method overloads
            // Order by name patterns
            .ThenBy(MemberNameOrdering);
        // TODO: decide how to order enum fields.
    }
}
