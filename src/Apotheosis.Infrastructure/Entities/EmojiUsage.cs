namespace Apotheosis.Infrastructure.Entities;

public sealed class EmojiUsage
{
    public int Id { get; set; }
    
    public ulong EmojiId { get; set; }
    
    public ulong GuildId { get; set; }
    
    public int Count { get; set; }
}