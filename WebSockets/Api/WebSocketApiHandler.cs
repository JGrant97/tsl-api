using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using tsl_api.Sessions;

namespace tsl_api.WebSockets.Api;

public class WebSocketApiHandler(ISessionService sessionService, IWebSocketRegistry socketRegistry) : IWebSocketApiHandler
{
    private static readonly TimeSpan PushInterval = TimeSpan.FromSeconds(1);

    private static readonly JsonSerializerOptions jsonConfig = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task HandleConnectionAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Expected WebSocket", cancellationToken);
            return;
        }

        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        var id = socketRegistry.Add(socket);

        try
        {
            await SendInitialSnapshotAsync(socket, cancellationToken);

            //Run sender and receiver concurrently
            var senderTask = RunPeriodicSenderAsync(socket, cancellationToken);
            var readerTask = RunReceiveLoopAsync(socket, cancellationToken);

            //Waits until either loop finishes.
            await Task.WhenAny(senderTask, readerTask);

            //Ensures the other loop has actually finished and all code inside both loops has run.
            await Task.WhenAll(senderTask, readerTask);
        }
        finally
        {
            await socketRegistry.RemoveAsync(id);
        }
    }

    //Sends the current session snapshot on initial connection
    private async Task SendInitialSnapshotAsync(WebSocket socket, CancellationToken cancellationToken) =>
        await socketRegistry.SendAsync(socket, SerializeSnapshot(), cancellationToken);

    //Sends the current session snapshot to the connected WebSocket client as a fixed interval
    private async Task RunPeriodicSenderAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                var payload = SerializeSnapshot();
                await socketRegistry.SendAsync(socket, payload, cancellationToken);
                await Task.Delay(PushInterval, cancellationToken);
            }
        }
        catch (OperationCanceledException) { /* normal on close */ }
    }

    //Continuously listens for incoming messages from the connected WebSocket client until the connection is closed or cancelled.
    private async Task RunReceiveLoopAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        //4kb receive buffer
        var buffer = new byte[4 * 1024];

        while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close)
                break;
        }
    }

    private string SerializeSnapshot() =>
        JsonSerializer.Serialize(new
        {
            type = MessageType.Snapshot.ToString(),
            data = sessionService.GetSnapshot()
        }, jsonConfig);
}
