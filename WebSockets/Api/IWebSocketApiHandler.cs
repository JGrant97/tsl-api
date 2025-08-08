using Microsoft.AspNetCore.Http.HttpResults;

namespace tsl_api.WebSockets.Api;

public interface IWebSocketApiHandler
{
    Task HandleConnectionAsync(HttpContext context, CancellationToken cancellationToken);
}
