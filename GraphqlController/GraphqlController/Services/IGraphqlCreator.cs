using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.Services
{
    public interface IGraphqlResolver
    {
        Task<T> CreateGraphqlEnityAsync<T, P>(P parameters, CancellationToken cancellationToken = default) where T : GraphNodeType<P>;
        Task<T> CreateGraphqlEnityAsync<T>(CancellationToken cancellationToken = default) where T : GraphNodeType;
    }
}
