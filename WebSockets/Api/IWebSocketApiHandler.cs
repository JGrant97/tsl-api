using Microsoft.AspNetCore.Http.HttpResults;

namespace tsl_api.WebSockets.Api;

public interface IWebSocketApiHandler
{
    Task<Results<Ok, BadRequest>> HandleConnectionAsync(HttpContext context);
}
