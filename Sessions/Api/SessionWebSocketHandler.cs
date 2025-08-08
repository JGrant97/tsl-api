using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using tsl_api.Sessions;
using tsl_api.WebSockets;

namespace tsl_api.Sessions.Api;

public class SessionWebSocketHandler(ISessionService sessionService, IWebSocketRegistry socketRegistry) 
    : WebSocketHandlerBase(socketRegistry), ISessionWebSocketApiHandler
{
    private readonly ISessionService _sessionService = sessionService;

    private static readonly JsonSerializerOptions JsonCfg = new()
    {
        Converters = { new JsonStringEnumConverter() } 
    };

    protected override TimeSpan PushInterval() => 
        TimeSpan.FromSeconds(1);

    protected override string InitialPayload() => 
        SerializeSnapshot();

    protected override string PeriodicPayload() =>
        SerializeSnapshot();

    private string SerializeSnapshot() =>
        JsonSerializer.Serialize(new
        {
            type = MessageType.Snapshot.ToString(),
            data = _sessionService.GetSnapshot()
        }, JsonCfg);
}
