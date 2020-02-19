using System;
using System.Threading.Tasks;

namespace GraphqlController
{

    public interface IGraphNodeType
    {

    }

    public abstract class GraphNodeType<T> : IGraphNodeType
    {
        public abstract Task OnCreateAsync(T param);
    }

}
