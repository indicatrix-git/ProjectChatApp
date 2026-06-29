namespace ChatApi.Dtos;

/// <summary>
/// Data transfer object returned by the REST API.
/// Keeps the API response independent from the database entity.
/// </summary>
public class MessageDto
{
    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
