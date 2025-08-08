using System.Net.WebSockets;

namespace tsl_api.WebSockets;

/// <summary>
/// Abstract WebSocket handler that implements the connections
/// </summary>
public abstract class WebSocketHandlerBase
{
    private readonly IWebSocketRegistry _socketRegistry;

    protected WebSocketHandlerBase(IWebSocketRegistry socketRegistry)
        => _socketRegistry = socketRegistry;

    //Serialized text to send immediately after connect.</summary>
    protected abstract string InitialPayload();

    //Serialized text to send on each tick.</summary>
    protected abstract string PeriodicPayload();

    //How often to send periodic updates (default 1s).</summary>
    protected virtual TimeSpan PushInterval() => TimeSpan.FromSeconds(1);

    //Initalises connection and begins loops
    public async Task HandleConnectionAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Expected WebSocket", cancellationToken);
            return;
        }

        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        var id = _socketRegistry.Add(socket);

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, context.RequestAborted);
        var ct = cancellationTokenSource.Token;

        try
        {
            //Initial payload
            await _socketRegistry.SendAsync(socket, InitialPayload(), ct);

            //Run sender and receiver concurrently
            var senderTask = RunPeriodicSenderAsync(socket, ct);
            var readerTask = RunReceiveLoopAsync(socket, ct);
            //Waits until either loop finishes.
            await Task.WhenAny(senderTask, readerTask);

            // Stop the other loop when one finishes
            await Task.WhenAny(senderTask, readerTask);
            cancellationTokenSource.Cancel();

            //Ensures the other loop has actually finished and all code inside both loops has run.
            try { await Task.WhenAll(senderTask, readerTask); } catch (OperationCanceledException) { /* expected */ }
        }
        finally
        {
            await _socketRegistry.RemoveAsync(id);
        }
    }

    //Sends the current session snapshot to the connected WebSocket client at a fixed interval
    private async Task RunPeriodicSenderAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await _socketRegistry.SendAsync(socket, PeriodicPayload(), cancellationToken);
                await Task.Delay(PushInterval(), cancellationToken);
            }
        }
        catch (OperationCanceledException) { /* normal on close */ }
    }

    //Continuously listens for incoming messages from the connected WebSocket client until the connection is closed or cancelled.
    private async Task RunReceiveLoopAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        // 4KB buffer
        var buffer = new byte[4 * 1024]; 
        try
        {
            while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close) break;
            }
        }
        catch (OperationCanceledException) { /* normal on close */ }
    }

}
