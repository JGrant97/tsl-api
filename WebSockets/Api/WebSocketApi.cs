namespace tsl_api.WebSockets.Api;

public static class WebSocketApi
{
    public static void MapWebSocketApi(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/ws");

        api.Map("/", async (HttpContext context, IWebSocketApiHandler handler, CancellationToken cancellationToken) => 
            await handler.HandleConnectionAsync(context, cancellationToken));
    }
}
