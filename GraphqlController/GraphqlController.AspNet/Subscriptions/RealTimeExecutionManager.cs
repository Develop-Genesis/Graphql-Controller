using GraphqlController.AspNetCore.Services;
using GraphqlController.AspNetCore.Subscriptions.Dtos;
using GraphqlController.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Subscriptions
{
    public class RealTimeExecutionManager : IRealTimeExecutionManager
    {
        Type _rootType;
        IServiceProvider _provider;

        Dictionary<string, OperationContext> _activeGraphqlOperations = new Dictionary<string, OperationContext>();

        public RealTimeExecutionManager(IServiceProvider provider, Type rootType)
        {
            _provider = provider;
            _rootType = rootType;
        }

        public event EventHandler<OperationMessage> NewOperationMessages;

        public void SendOperationMessage(OperationMessage operationMessage)
        {
            switch(operationMessage.Type)
            {
                case OperationType.GraphqlStart:
                    StartOperation(operationMessage, _provider);
                    break;

                case OperationType.GraphqlStop:
                    CancelOperation(operationMessage.Id);
                    break;
            }
        }

        void StartOperation(OperationMessage message, IServiceProvider serviceProvider)
        {
            var tokenSource = new CancellationTokenSource();
            var serviceScope = serviceProvider.CreateScope();
            _activeGraphqlOperations.Add(message.Id, new OperationContext(message.Id, serviceScope, tokenSource));
            ExecuteOperationAsync(message.Id, message.Payload.ToObject<GraphQlRequest>(), serviceScope.ServiceProvider, tokenSource.Token);
        }

        void CancelOperation(string operationId)
        {
            OperationContext operationContext;
            if(_activeGraphqlOperations.TryGetValue(operationId, out operationContext))
            {
                operationContext.CancellationTokenSource.Cancel();
                operationContext.ServiceScope.Dispose();
            }
        }

        void ExecuteOperationAsync(string id, GraphQlRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var executor = serviceProvider.GetService<IGraphQLExecutor>();
            var executionBuilderResolver = serviceProvider.GetService<IExecutionBuilderResolver>();

            var realtimeExecuter = new RealTimeQueryExecuter(executionBuilderResolver, executor, _rootType, id);

            realtimeExecuter.Execute(request, executerCallback, cancellationToken);
        }

        private void executerCallback(OperationMessage message)
        {
            if(message.Type == OperationType.GraphqlError || message.Type == OperationType.GraphqlComplete)
            {
                CancelOperation(message.Id);
            }

            OnNewOperationMessage(message);
        }

        void OnNewOperationMessage(OperationMessage message)
             => NewOperationMessages?.Invoke(this, message);

        public void Dispose()
        {
            foreach(var operationContext in _activeGraphqlOperations.Values)
            {
                operationContext.CancellationTokenSource.Cancel();
                operationContext.ServiceScope.Dispose();
            }
        }
    }

    public class OperationContext
    {
        public OperationContext(string id, IServiceScope serviceScope, CancellationTokenSource cancellationTokenSource)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ServiceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
            CancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        public string Id { get; }
        public IServiceScope ServiceScope { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
    }

}
