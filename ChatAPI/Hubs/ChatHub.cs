using ChatApi.Data;
using ChatApi.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApi.Hubs;

/// <summary>
/// SignalR hub that handles real-time chat communication.
/// </summary>
public class ChatHub : Hub
{
    private readonly ChatContext _context;

    public ChatHub(ChatContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Called by a client to send a message.
    /// 1) validates the message
    /// 2) saves it into MySQL
    /// 3) broadcasts it to every connected client via "ReceiveMessage"
    /// </summary>
    public async Task SendMessage(string sender, string text)
    {
        // --- 1. Validation ---
        if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(text))
            return;

        sender = sender.Trim();
        text = text.Trim();

        if (sender.Length > 50) sender = sender[..50];
        if (text.Length > 500) text = text[..500];

        // --- 2. Persist to database ---
        var message = new Message
        {
            Sender = sender,
            Text = text,
            SentAt = DateTime.Now
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // --- 3. Broadcast to all connected clients ---
        await Clients.All.SendAsync("ReceiveMessage", message.Sender, message.Text, message.SentAt);
    }
}
