using tsl_api.Sessions.Api;
using tsl_api.WebSockets;

namespace tsl_api.Sessions;

public static class ServiceCollectionExtensions
{
    public static void AddSessionServices(this IServiceCollection services)
    {
        //Is singleton because its in memeory, would be using scoped with EF Core db
        services.AddSingleton<ISessionService, MockSessionService>();
        services.AddSingleton<ISessionWebSocketApiHandler, SessionWebSocketHandler>();
    }
}
