using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.Services
{
    public interface IGraphqlCreator
    {
        Task<T> CreateGraphqlEnityAsync<T, P>(P parameters) where T : GraphNodeType<P>;
    }
}
