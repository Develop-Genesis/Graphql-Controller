using GraphqlController.AspNetCore.Subscriptions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Subscriptions
{
    public interface IRealTimeExecutionManager
    {
        void SendOperationMessage(OperationMessage operationMessage);

        event EventHandler<OperationMessage> NewOperationMessages;
    }
}
