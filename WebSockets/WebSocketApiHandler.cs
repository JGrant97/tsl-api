using Microsoft.AspNetCore.Http.HttpResults;

namespace tsl_api.WebSockets;

public class WebSocketApiHandler : IWebSocketApiHandler
{
    public Task<Ok> Add(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
