using System.Net.WebSockets;

namespace tsl_api.WebSockets;

public interface IWebSocketRegistry
{
    Guid Add(WebSocket socket);

    Task RemoveAsync(Guid id);

    Task SendAsync(WebSocket socket, string text, CancellationToken cancellationToken = default);
}
