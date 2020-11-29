using GraphqlController;
using GraphqlController.Attributes;
using GraphqlController.Helpers;
using Gski.Relay.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Relay
{
    public class InjectableNode<T> : GraphNodeType<T>, INode
    {
        public GlobalId Id { get; private set; }

        protected void SetId(string id)
        {
            var name = GetEntityName();
            Id = new GlobalId(id, name);
        }

        protected string GetEntityName()
        {
            var type = this.GetType();
            var nameAttr = type.GetAttribute<NameAttribute>();
            var name = nameAttr?.Name ?? type.Name;
            return name;
        }
    }


    public class Node : INode
    {
        public GlobalId Id { get; private set; }

        protected void SetId(string id)
        {
            var name = GetEntityName();
            Id = new GlobalId(id, name);
        }

        protected string GetEntityName()
        {
            var type = this.GetType();
            var nameAttr = type.GetAttribute<NameAttribute>();
            var name = nameAttr?.Name ?? type.Name;
            return name;
        }
    }


    public class InjectableNode : InjectableNode<object>
    {

    }
}
