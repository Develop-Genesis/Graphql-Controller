using GraphqlController.AspNetCore.Subscriptions.Dtos;
using GraphqlController.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Subscriptions.WebSockets
{
    public class WebSocketsMiddleware
    {
        RequestDelegate _next;
        PathString _path;
        Type rootType;
        bool isConnectionInitialized = false;

        public WebSocketsMiddleware(RequestDelegate next, PathString path, Type rootType)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IScopedServiceProviderResolver scopedServiceProviderResolver)
        {
            // If its not a ws or the path does not match ignore
            if(!httpContext.Request.Path.StartsWithSegments(_path) || !httpContext.WebSockets.IsWebSocketRequest)
            {
                await _next(httpContext);
                return;
            }

            var socket = await httpContext.WebSockets.AcceptWebSocketAsync("graphql-ws");

            if (!httpContext.WebSockets.WebSocketRequestedProtocols.Contains(socket.SubProtocol))
            {
                await socket.CloseAsync(
                        WebSocketCloseStatus.ProtocolError,
                        "Server only supports graphql-ws protocol",
                        httpContext.RequestAborted);

                return;
            }

            var serviceProvider = scopedServiceProviderResolver.GetProvider();
            var realTimeExecutionManager = new RealTimeExecutionManager(serviceProvider, rootType);

            await RunConnectionAsync(realTimeExecutionManager, socket, default);
        }

        static async Task RunConnectionAsync(IRealTimeExecutionManager realTimeExecutionManager, WebSocket webSocket, CancellationToken cancellationToken)
        {
            realTimeExecutionManager.NewOperationMessages += async (o, e) => await SendSocketMessageAsync(webSocket, e, default);

            var operationMessage = await GetSocketMessageAsync(webSocket, cancellationToken);

            switch(operationMessage.Type)
            {
                case OperationType.GraphqlConnectionInit:
                    await SendSocketMessageAsync(webSocket, new OperationMessage()
                    {
                        Type = OperationType.GraphqlConnectionAck
                    }, cancellationToken);
                    break;

                case OperationType.GraphqlConnectionTerminate:
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", default);
                    return;

                default:
                    realTimeExecutionManager.SendOperationMessage(operationMessage);
                    break;
            }

        }

        static async Task<OperationMessage> GetSocketMessageAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            const int maxFrameSize = 1024;

            var reader = new WebSocketReader(webSocket, maxFrameSize);

            using (var ms = new MemoryStream(maxFrameSize))
            {
                await reader.ReadAsync(ms, cancellationToken);
                ms.Seek(0, SeekOrigin.Begin);

                using(var sr = new StreamReader(ms))
                using(var jsonReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();

                    return serializer.Deserialize<OperationMessage>(jsonReader);
                }
            }

        }

        static async Task SendSocketMessageAsync(WebSocket socket, OperationMessage message, CancellationToken cancellationToken)
        {
            using(var ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, message);

                ms.Seek(0, SeekOrigin.Begin);

                await socket.SendAsync(new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length), WebSocketMessageType.Text, true, cancellationToken);
            }
        }

    }
}
