using Gski.Relay.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Relay
{
    public interface INodeResolver
    {
        public Task<INode> GetByIdAsync(GlobalId id, CancellationToken cancellationToken);
    }

    public abstract class NodeResolver<T> : INodeResolver where T : INode
    {
        public abstract Task<T> GetByIdAsync(string id, CancellationToken cancellationToken);

        public async Task<INode> GetByIdAsync(GlobalId id, CancellationToken cancellationToken)
        {
            return await GetByIdAsync(id.Id, cancellationToken);
        }
    }
}
