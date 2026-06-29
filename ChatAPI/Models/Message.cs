using System.ComponentModel.DataAnnotations;

namespace ChatApi.Models;

/// <summary>
/// Database entity representing one chat message.
/// Maps to the "Messages" table in the "ChatApp" database.
/// </summary>
public class Message
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Sender { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
}
