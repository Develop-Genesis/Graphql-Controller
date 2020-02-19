using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphqlController.GraphQl
{
    public class DynamicEnumerationGraphType : EnumerationGraphType
    {
        public DynamicEnumerationGraphType(Type enumType)
        {
            var type = enumType;
            var names = Enum.GetNames(type);
            var enumMembers = names.Select(n => (name: n, member: type
                    .GetMember(n, BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .First()));
            var enumGraphData = enumMembers.Select(e => (
                name: ChangeEnumCase(e.name),
                value: Enum.Parse(type, e.name),
                description: e.member.Description(),
                deprecation: e.member.ObsoleteMessage()
            ));

            Name = StringUtils.ToPascalCase(type.Name);
            Description ??= type.Description();
            DeprecationReason ??= type.ObsoleteMessage();

            foreach (var (name, value, description, deprecation) in enumGraphData)
            {
                AddValue(name, description, value, deprecation);
            }
        }

        private string ChangeEnumCase(string val) => StringUtils.ToConstantCase(val);
    }
}
