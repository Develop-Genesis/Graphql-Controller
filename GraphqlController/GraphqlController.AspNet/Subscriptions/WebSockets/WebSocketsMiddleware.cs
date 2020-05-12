using GraphqlController.AspNetCore.Subscriptions.Dtos;
using GraphqlController.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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

            await RunConnectionAsync(realTimeExecutionManager, socket);
        }

        async Task RunConnectionAsync(IRealTimeExecutionManager realTimeExecutionManager, WebSocket socket)
        {
            realTimeExecutionManager.NewOperationMessages += async (o, e) => await SendSocketMessageAsync(socket, e, default);



        }

        static async Task<OperationMessage> GetSocketMessageAsync(WebSocket socket, CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<byte>(  )
            socket.ReceiveAsync()
        }

        static async Task SendSocketMessageAsync(WebSocket socket, OperationMessage message, CancellationToken cancellationToken)
        {

        }

    }
}
