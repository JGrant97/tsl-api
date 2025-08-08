using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.WebSockets;
using System.Text.Json;
using tsl_api.Sessions;

namespace tsl_api.WebSockets.Api;

public class WebSocketApiHandler(ISessionService sessionService, IWebSocketRegistry socketRegistry) : IWebSocketApiHandler
{
    public async Task<Results<Ok, BadRequest>> HandleConnectionAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
            return TypedResults.BadRequest();

        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        var id = socketRegistry.Add(socket);

        try
        {
            // optional: send initial snapshot immediately
            var snapshot = JsonSerializer.Serialize(new MessageDto("snapshot", sessionService.GetSnapshot().ToList()));
            await socketRegistry.SendAsync(socket, snapshot, context.RequestAborted);

            // read loop (ignore messages; exit on close)
            var buffer = new byte[4 * 1024];
            while (socket.State == WebSocketState.Open && !context.RequestAborted.IsCancellationRequested)
            {
                var result = await socket.ReceiveAsync(buffer, context.RequestAborted);
                if (result.MessageType == WebSocketMessageType.Close) break;
                // You could handle client pings/commands here if needed.
            }

            return TypedResults.Ok();
        }
        finally
        {
            await socketRegistry.RemoveAsync(id);
        }
    }
}
