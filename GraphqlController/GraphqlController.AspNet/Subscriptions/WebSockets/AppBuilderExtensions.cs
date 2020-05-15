using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Subscriptions.WebSockets
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseGraphqlWebSocketProtocol(this IApplicationBuilder app, Type root, string path)
        {
            app.UseMiddleware<WebSocketsMiddleware>(new PathString(path), root);
            return app;
        }

        public static IApplicationBuilder UseGraphqlWebSocketProtocol<T>(this IApplicationBuilder app, string path)
            => UseGraphqlWebSocketProtocol(app, typeof(T), path);

    }
}
