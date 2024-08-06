using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Chatbot.Services.Interfaces
{
    public interface IWebSocketHandlerService
    {
        Task HandleWebSocket(ConcurrentDictionary<string, WebSocket> webSockets, WebSocket webSocket, string socketId, Dictionary<string, string> requestHeaders);
    }
}