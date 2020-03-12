using System;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController
{

    public interface IGraphNodeType
    {

    }

    public abstract class GraphNodeType<T> : IGraphNodeType
    {
        public virtual Task OnCreateAsync(T param, CancellationToken cancellationToken) => Task.CompletedTask;        
    }

    public abstract class GraphNodeType : GraphNodeType<object>
    {
        public override Task OnCreateAsync(object param, CancellationToken cancellationToken)
        {
            OnCreateAsync(cancellationToken);
            return base.OnCreateAsync(param, cancellationToken);
        }

        public virtual Task OnCreateAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
