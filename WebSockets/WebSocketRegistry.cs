using System.Net.WebSockets;
using System.Text;

namespace tsl_api.WebSockets;

/// <summary>
/// Manages and stores web socket connections. 
/// provides add, remove and send functionality
/// </summary>
public class WebSocketRegistry : IWebSocketRegistry
{
    private readonly Dictionary<Guid, WebSocket> _clients = new();

    public Guid Add(WebSocket socket)
    {
        var id = Guid.NewGuid();
        _clients[id] = socket;
        return id;
    }

    public async Task RemoveAsync(Guid id)
    {
        if (_clients.Remove(id, out var socket))
        {
            try { 
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None); 
            }
            catch { /* ignore */ }
            socket.Dispose();
        }
    }

    public async Task SendAsync(WebSocket socket, string text, CancellationToken cancellationToken = default) =>
        await socket.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text,true, cancellationToken);
}
