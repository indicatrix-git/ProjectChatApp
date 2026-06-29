using ChatApi.Data;
using ChatApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Controllers;

/// <summary>
/// REST API used by clients to load chat history when they join.
/// </summary>
[ApiController]
[Route("api/[controller]")] // => /api/messages
public class MessagesController : ControllerBase
{
    private readonly ChatContext _context;

    public MessagesController(ChatContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/messages
    /// Returns the latest 50 messages ordered chronologically (oldest first).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages()
    {
        var messages = await _context.Messages
            .OrderByDescending(m => m.SentAt)   // take the newest 50...
            .Take(50)
            .OrderBy(m => m.SentAt)             // ...then show them oldest -> newest
            .Select(m => new MessageDto
            {
                Sender = m.Sender,
                Text = m.Text,
                SentAt = m.SentAt
            })
            .ToListAsync();

        return Ok(messages);
    }
}
