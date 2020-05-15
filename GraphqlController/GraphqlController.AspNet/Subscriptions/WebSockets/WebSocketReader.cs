using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Subscriptions.WebSockets
{
    public class WebSocketReader
    {
        int _frameMaxSize = 1024;
        WebSocket _webSocket;

        public WebSocketReader(WebSocket webSocket, int frameMaxSize = 1024)
        {
            _frameMaxSize = frameMaxSize;
            _webSocket = webSocket;
        }

        public async Task ReadAsync(Stream stream, CancellationToken cancellationToken)
        {
            var segment = new ArraySegment<byte>(new byte[_frameMaxSize]);

            WebSocketReceiveResult result = null;

            do
            {
                result = await _webSocket.ReceiveAsync(segment, cancellationToken);
                await stream.WriteAsync(segment.Array, 0, result.Count);
                
            } while (result != null && !result.EndOfMessage);
        }

    }
}
