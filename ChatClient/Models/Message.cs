using System;

namespace ChatClient.Models;

/// <summary>
/// Client-side model for a single chat message.
/// </summary>
public class Message
{
    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Display header, e.g. "Peter (14:22)".
    /// </summary>
    public string Header => $"{Sender} ({SentAt:HH:mm})";
}
