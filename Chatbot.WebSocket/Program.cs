using Chatbot.Services.Interfaces;
using Chatbot.Services.Services;
using System.Collections.Concurrent;
using System.Net.WebSockets;

ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IWebSocketHandlerService, WebSocketHandlerService>();
builder.WebHost.UseUrls("http://localhost:6969");

var app = builder.Build();
app.UseWebSockets();

var webSocketHandlerService = app.Services.GetRequiredService<IWebSocketHandlerService>();

app.Use(async (context, next) =>
{
    try
    {
        if (context.Request.Path == "/chat")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                string socketId = Guid.NewGuid().ToString();
                _sockets.TryAdd(socketId, webSocket);

                Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
                if (context != null && context.Request != null && context.Request.Headers != null && context.Request.Headers.Any())
                {
                    requestHeaders = context?.Request?.Headers.ToDictionary(header => header.Key, header => header.Value.ToString()) ?? new Dictionary<string, string>();
                }

                await webSocketHandlerService.HandleWebSocket(_sockets, webSocket, socketId, requestHeaders);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
        else
        {
            await context.Response.WriteAsync("Web service for Chatbot is running.");
            await next();
        }
    }
    catch (Exception ex)
    {
        await context.Response.WriteAsync($"Error occurred in the web service. : {ex.Message}");
    }
});

await app.RunAsync();