namespace tsl_api.WebSockets;

public static class WebSocketApi
{
    public static void MapWebSocketApi(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/ws");

        api.Map("/", async (HttpContext context, IWebSocketApiHandler handler) => await handler.Add(context));
    }
}
