namespace tsl_api.Sessions.Api;

public static class SessionApi
{
    public static void MapSessionApi(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/session");

        //Generic REST endpoints here

        var webSockets = api.MapGroup("/ws");

        webSockets.Map("/", async (HttpContext context, ISessionWebSocketApiHandler handler, CancellationToken cancellationToken) => 
            await handler.HandleConnectionAsync(context, cancellationToken));
    }
}
