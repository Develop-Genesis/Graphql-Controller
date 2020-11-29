using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Relay.Types
{    
    public interface INode
    {
        public GlobalId Id { get; }
    }
}
