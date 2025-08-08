using System.Net.WebSockets;

namespace tsl_api.WebSockets;

public class WebSocketRegistry : IWebSocketRegistry
{
    public Guid Add(WebSocket socket)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SendAsync(WebSocket socket, string text, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
