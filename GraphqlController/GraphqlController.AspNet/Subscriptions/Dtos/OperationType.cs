using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Subscriptions.Dtos
{
    public static class OperationType
    {
        public const string GraphqlConnectionInit = "GQL_CONNECTION_INIT";

        public const string GraphqlStart = "GQL_START";

        public const string GraphqlStop = "GQL_STOP";

        public const string GraphqlConnectionError = "GQL_CONNECTION_ERROR";

        public const string GraphqlConnectionAck = "GQL_CONNECTION_ACK";

        public const string GraphqlData = "GQL_DATA";

        public const string GraphqlError = "GQL_ERROR";

        public const string GraphqlComplete = "GQL_COMPLETE";

        public const string GraphqlConnectionKeepAlive = "GQL_CONNECTION_KEEP_ALIVE";
    }
}
