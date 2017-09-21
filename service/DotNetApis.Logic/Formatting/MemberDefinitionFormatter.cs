using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Cecil;
using DotNetApis.Structure.Entities;
using Mono.Cecil;

namespace DotNetApis.Logic.Formatting
{
    public sealed class MemberDefinitionFormatter
    {
        private readonly MethodFormatter _methodFormatter;
        private readonly PropertyFormatter _propertyFormatter;
        private readonly EventFormatter _eventFormatter;
        private readonly FieldFormatter _fieldFormatter;
        private readonly EnumFormatter _enumFormatter;
        private readonly DelegateFormatter _delegateFormatter;
        private readonly TypeFormatter _typeFormatter;

        public MemberDefinitionFormatter(MethodFormatter methodFormatter, PropertyFormatter propertyFormatter, EventFormatter eventFormatter, FieldFormatter fieldFormatter,
            EnumFormatter enumFormatter, DelegateFormatter delegateFormatter, TypeFormatter typeFormatter)
        {
            _methodFormatter = methodFormatter;
            _propertyFormatter = propertyFormatter;
            _eventFormatter = eventFormatter;
            _fieldFormatter = fieldFormatter;
            _enumFormatter = enumFormatter;
            _delegateFormatter = delegateFormatter;
            _typeFormatter = typeFormatter;
        }

        /// <summary>
        /// Formats a member definition.
        /// </summary>
        /// <param name="member">The member to format.</param>
        public IEntity MemberDefinition(IMemberDefinition member)
        {
            if (member is MethodDefinition method)
                return _methodFormatter.Method(method);
            if (member is PropertyDefinition property)
                return _propertyFormatter.Property(property);
            if (member is EventDefinition @event)
                return _eventFormatter.Event(@event);
            if (member is FieldDefinition field)
                return _fieldFormatter.Field(field);
            var type = (TypeDefinition)member;
            if (type.IsEnum)
                return _enumFormatter.Enum(type);
            if (type.IsDelegate())
                return _delegateFormatter.Delegate(type);
            return _typeFormatter.Type(type, MemberDefinition);
        }
    }
}
