using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatClient.Services;

/// <summary>
/// Wraps the SignalR connection: connecting, sending and receiving messages.
/// </summary>
public class SignalRService
{
    private readonly HubConnection _connection;

    /// <summary>Raised whenever the server broadcasts a message (sender, text, sentAt).</summary>
    public event Action<string, string, DateTime>? MessageReceived;

    public SignalRService(string hubUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        // Subscribe to the broadcast method defined on the server hub.
        _connection.On<string, string, DateTime>("ReceiveMessage", (sender, text, sentAt) =>
        {
            MessageReceived?.Invoke(sender, text, sentAt);
        });
    }

    public async Task ConnectAsync()
    {
        if (_connection.State == HubConnectionState.Disconnected)
            await _connection.StartAsync();
    }

    public async Task SendMessageAsync(string sender, string text)
    {
        await _connection.InvokeAsync("SendMessage", sender, text);
    }

    public async Task DisconnectAsync()
    {
        await _connection.StopAsync();
    }
}
