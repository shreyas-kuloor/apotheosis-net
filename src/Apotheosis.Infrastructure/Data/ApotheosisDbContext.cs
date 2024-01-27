using Apotheosis.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apotheosis.Infrastructure.Data;

public class ApotheosisDbContext(DbContextOptions<ApotheosisDbContext> options) : DbContext(options)
{
    public DbSet<EmojiUsage> EmojiUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmojiUsage>()
            .HasIndex(e => new { e.EmojiId, e.GuildId })
            .IsUnique();
    }
}