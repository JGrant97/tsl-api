using tsl_api.Sessions;
using tsl_api.WebSockets.Api;

namespace tsl_api.WebSockets;

public static class ServiceCollectionExtensions
{
    public static void AddWebSocketServices(this IServiceCollection services)
    {
        //Is singleton because its in memeory, would be using scoped with EF Core db
        services.AddSingleton<ISessionService, MockSessionService>();

        services.AddSingleton<IWebSocketRegistry, WebSocketRegistry>();
        services.AddSingleton<IWebSocketApiHandler, WebSocketApiHandler>();
    }
}
