using GraphQL.Subscription;
using GraphqlController.AspNetCore.Services;
using GraphqlController.AspNetCore.Subscriptions.Dtos;
using GraphqlController.Execution;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;

namespace GraphqlController.AspNetCore.Subscriptions
{
    public class RealTimeQueryExecuter
    {
        IGraphQLExecutor _executor;
        IExecutionBuilderResolver _executionBuilderResolver;
        
        Type _root;
        string _operationId;

        public RealTimeQueryExecuter(IExecutionBuilderResolver executionBuilderResolver, IGraphQLExecutor executor, Type root, string operationId)
        {
            _executionBuilderResolver = executionBuilderResolver;
            _executor = executor;
            _operationId = operationId;
        }

        public void Execute(GraphQlRequest request, Action<OperationMessage> callback, CancellationToken cancellationToken)
        {
            var executionBuilder = _executionBuilderResolver.GetGraphqlExecutionBuilder(_root);

            _executor.ExecuteAsync(executionBuilder, request, _root, new ExecutionDataDictionary() {
                { "IsRealtimeRequest", true }
            }, cancellationToken)
            .ContinueWith(result =>
           {
               if(result.IsFaulted)
               {
                   callback(new OperationMessage()
                   {
                       Id = _operationId,
                       Type = OperationType.GraphqlError,
                       Payload = new Newtonsoft.Json.Linq.JValue(result.Exception.Message)
                   });
                  ;
               }

               var executionData = result.Result.ExecutionData;
               var executionResult = result.Result.ExecutionResult;

               if(executionResult is SubscriptionExecutionResult subscriptionExecutionResult)
               {
                    if (subscriptionExecutionResult.Streams?.Values.SingleOrDefault() == null)
                    {
                        callback(new OperationMessage()
                        {
                            Id = _operationId,
                            Payload = JToken.FromObject(executionResult.ToResultDictionary()) as JValue,
                            Type = OperationType.GraphqlError
                        });
                    }
                    else
                    {
                       var stream = subscriptionExecutionResult.Streams.Values.Single();

                       stream.Synchronize().Subscribe(
                           onNext: (data) => callback(new OperationMessage()
                           {
                               Id = _operationId,
                               Payload = JToken.FromObject(data.ToResultDictionary()) as JValue,
                               Type = OperationType.GraphqlData
                           }),
                           onError: (error) => callback(new OperationMessage()
                           {
                               Id = _operationId,
                               Payload = new JValue(error.Message),
                               Type = OperationType.GraphqlError
                           }),
                           onCompleted: () => callback(new OperationMessage()
                           {
                               Id = _operationId,                               
                               Type = OperationType.GraphqlComplete
                           }),
                           token: cancellationToken
                          );
                   }
               }
               else
               {
                   var dictionary = executionResult.ToResultDictionary();
                   callback(new OperationMessage()
                   {
                       Id = _operationId,
                       Payload = JToken.FromObject(dictionary) as JValue,
                       Type = OperationType.GraphqlData
                   });

                   callback(new OperationMessage()
                   {
                       Id = _operationId,
                       Type = OperationType.GraphqlComplete
                   });

               }

           });

        }

    }
}
