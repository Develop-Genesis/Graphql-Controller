using GraphQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public class GraphQLExecutionBuilder : IGraphQLExecutionBuilder
    {

        List<(Func<ExecutionContext, Func<Task>, Task> MidlewareDelegate, Type MidlewareType)> _midlewares
            = new List<(Func<ExecutionContext, Func<Task>, Task> MidlewareDelegate, Type MidlewareType)>();

        public IGraphqlExecution BuildExecution(
            GraphQlRequest request, 
            DocumentExecuterMidleware documentExecuterMidleware, 
            IExecutionDataDictionary data,
            IServiceProvider serviceProvider, 
            CancellationToken cancellationToken)
        {
            var context = new ExecutionContext(request, data, cancellationToken);

            var midlewares = _midlewares.Select(x =>
            {
                if(x.MidlewareType != null)
                {
                    return serviceProvider.GetService(x.MidlewareType) as IExecutionMiddleware;
                }

                return new MidlewareDelegateWrapper(x.MidlewareDelegate);
            }).Append(documentExecuterMidleware);

            return new GraphQLExecution(context, midlewares);
        }

        public IGraphQLExecutionBuilder Use<T>() where T : IExecutionMiddleware
        {
            _midlewares.Add((MidlewareDelegate: null, MidlewareType: typeof(T)));
            return this;
        }

        public IGraphQLExecutionBuilder Use(Func<ExecutionContext, Func<Task>, Task> middlewareDelegate)
        {
            _midlewares.Add((MidlewareDelegate: middlewareDelegate, MidlewareType: null));
            return this;
        }
    }

    public class GraphQLExecution : IGraphqlExecution
    {
        ExecutionContext _context;
        IEnumerable<IExecutionMiddleware> _middlewares;

        public GraphQLExecution(ExecutionContext context, IEnumerable<IExecutionMiddleware> middlewares)
        {
            _context = context;
            _middlewares = middlewares;
        }

        public async Task<GraphqlControllerExecutionResult> ExecuteAsync()
        {
            var enumerator = _middlewares.GetEnumerator();

            enumerator.MoveNext();
            var first = enumerator.Current;

            await first.ExecuteAsync(_context, new Func<Task>(()=>Next(enumerator)));

            return new GraphqlControllerExecutionResult(_context.Result, _context.ExecutionData);
        }

        private async Task Next(IEnumerator<IExecutionMiddleware> enumerator)
        {
            if(enumerator.MoveNext())
            {
                var current = enumerator.Current;
                await current.ExecuteAsync(_context, () => Next(enumerator));
            }
        }
    }



    public class MidlewareDelegateWrapper : IExecutionMiddleware
    {
        Func<ExecutionContext, Func<Task>, Task> _middlewareDelegate;

        public MidlewareDelegateWrapper(Func<ExecutionContext, Func<Task>, Task> middlewareDelegate)
        {
            _middlewareDelegate = middlewareDelegate;
        }

        public Task ExecuteAsync(ExecutionContext executionContext, Func<Task> next)
          => _middlewareDelegate.Invoke(executionContext, next);
    }

}
