using Microsoft.AspNetCore.Http.HttpResults;

namespace tsl_api.WebSockets;

public interface IWebSocketApiHandler
{
    Task<Ok> Add(HttpContext context);
}
