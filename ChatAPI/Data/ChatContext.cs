using ChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Data;

/// <summary>
/// Entity Framework Core database context for the chat application.
/// </summary>
public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Sender).HasMaxLength(50).IsRequired();
            entity.Property(m => m.Text).HasMaxLength(500).IsRequired();
            entity.Property(m => m.SentAt).IsRequired();
        });
    }
}
