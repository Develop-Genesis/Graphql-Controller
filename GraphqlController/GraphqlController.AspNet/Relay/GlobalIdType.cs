using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using Gski.Relay.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Relay
{
    public class GlobalIdType : ScalarGraphType
    {
        public GlobalIdType()
        {
            Name = "GlobalId";
            Description = "Represent an id of an entity globaly";
        }

        public override object ParseLiteral(IValue value)
        {
            if (value is GlobalIdValueNode globalIdValue)
                return ParseValue(globalIdValue.Value);

            return value is StringValue stringValue
                         ? ParseValue(stringValue.Value)
                         : null;
        }

        public override object ParseValue(object value)
        {
            return ValueConverter.ConvertTo(value, typeof(GlobalId));
        }

        public override object Serialize(object value)
        {
            return value switch
            {
                string s => ValueConverter.ConvertTo(value, typeof(GlobalId)),
                GlobalId v => v.Deserialize(),
                _ => throw new NotImplementedException()
            };            
        }
    }

    public class GlobalIdValueNode : ValueNode<GlobalId>
    {

        public GlobalIdValueNode(GlobalId value)
        {
            Value = value;
        }

        protected override bool Equals(ValueNode<GlobalId> node)
        {
            return Value.Equals(node);
        }
    }

    public class GlobalIdAstValueConverter : IAstFromValueConverter
    {
        public IValue Convert(object value, IGraphType type)
        {
            return new GlobalIdValueNode(value as GlobalId);
        }

        public bool Matches(object value, IGraphType type)
        {
            return value is GlobalId;
        }
    }

}
