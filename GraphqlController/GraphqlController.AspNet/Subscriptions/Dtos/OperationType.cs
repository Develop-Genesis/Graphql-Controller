using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Subscriptions.Dtos
{
    public static class OperationType
    {
        public const string GraphqlConnectionInit = "connection_init";

        public const string GraphqlStart = "start";

        public const string GraphqlStop = "stop";

        public const string GraphqlConnectionError = "connection_error";

        public const string GraphqlConnectionAck = "connection_ack";

        public const string GraphqlData = "data";

        public const string GraphqlError = "error";

        public const string GraphqlComplete = "complete";

        public const string GraphqlConnectionKeepAlive = "ka";

        public const string GraphqlConnectionTerminate = "connection_terminate";
    }
}