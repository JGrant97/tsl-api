namespace tsl_api.WebSockets;

public static class ServiceCollectionExtensions
{
    public static void AddWebSocketServices(this IServiceCollection services)
    {
        services.AddScoped<IWebSocketApiHandler, WebSocketApiHandler>();
    }
}
