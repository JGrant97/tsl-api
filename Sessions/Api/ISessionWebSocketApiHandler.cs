using Microsoft.AspNetCore.Http.HttpResults;

namespace tsl_api.Sessions.Api;

public interface ISessionWebSocketApiHandler
{
    Task HandleConnectionAsync(HttpContext context, CancellationToken cancellationToken);
}
