namespace Apotheosis.Core.Features.EmojiCounter.Aggregates;

public sealed class EmojiUsage
{
    public int Id { get; set; }

    public string? Name { get; set; }
    
    public ulong EmojiId { get; set; }
    
    public ulong GuildId { get; set; }
    
    public int Count { get; set; }
}