namespace Apotheosis.Core.Features.EmojiCounter.Models;

public class EmojiCounterDto
{
    public string? Name { get; set; }

    public ulong EmojiId { get; set; }

    public ulong GuildId { get; set; }

    public int Count { get; set; }
}
