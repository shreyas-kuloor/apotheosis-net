using Apotheosis.Core.Features.EmojiCounter.Aggregates;
using Apotheosis.Core.Features.ReactionForwarding.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Apotheosis.Infrastructure.Data;

public class ApotheosisDbContext(DbContextOptions<ApotheosisDbContext> options) : DbContext(options)
{
    public DbSet<EmojiUsage> EmojiUsages { get; set; }
    public DbSet<ReactionForwardingRule> ReactionForwardingRules { get; set; }
    public DbSet<ForwardedMessage> ForwardedMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmojiUsage>()
            .HasIndex(e => new { e.EmojiId, e.GuildId })
            .IsUnique();
        
        modelBuilder.Entity<ReactionForwardingRule>()
            .HasIndex(e => new { e.EmojiId, e.Name, e.ChannelId })
            .IsUnique();

        modelBuilder.Entity<ReactionForwardingRule>()
            .HasIndex(e => new { e.EmojiId, e.Name});
        
        modelBuilder.Entity<ForwardedMessage>()
            .HasIndex(e => new { e.MessageId, e.ChannelId })
            .IsUnique();
    }
}