using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Main;

public class Program
{
    const int BUFF_SIZE = 1000;
    byte[] buffer = new byte[BUFF_SIZE];
    int running_exchanges = 0;
    int last_reported = 0;

    /// <summary>
    /// Serves incomming WS connections.
    /// Receive sent data and send back the response
    /// </summary>
    public async Task HandleWsRequest(
        CancellationToken token,
        ILoggerFactory loggerFactory,
        HttpContext context
    )
    {
        var logger = loggerFactory.CreateLogger("HandleWs");
        try
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        Interlocked.Increment(ref running_exchanges);
                        var segment = new ArraySegment<byte>(buffer, 0, BUFF_SIZE);
                        var result = await webSocket.ReceiveAsync(segment, token);
                        var received = UnicodeEncoding.UTF8.GetString(buffer, 0, result.Count);
                        //logger.LogDebug("Received: " + received);

                        //await Task.Delay(1000);
                        await webSocket.SendAsync(
                            Encoding.ASCII.GetBytes(
                                $"Response - {DateTime.Now} Received: {received}"
                            ),
                            WebSocketMessageType.Text,
                            true,
                            token
                        );
                        var last_reported_value = Volatile.Read(ref last_reported);
                        var second = DateTime.Now.Second;
                        if (second != last_reported_value)
                        {
                            var running_requests_value = Volatile.Read(ref running_exchanges);
                            logger.LogDebug("Exchanges per second:" + running_requests_value);
                            Volatile.Write(ref last_reported, DateTime.Now.Second);
                            Volatile.Write(ref running_exchanges,0);
                        }
                        

                        // When function finishes, WS is automatically closed by sending finish and closing socket.
                    }
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e.Message);
        }
    }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://localhost:5001");
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddConsole();

        var app = builder.Build();
        app.Logger.LogDebug("Starting..");

        var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) };

        app.UseWebSockets(webSocketOptions);

        var handler = new Program();
        app.Map("/ws", handler.HandleWsRequest);
        await app.RunAsync();
    }
}
