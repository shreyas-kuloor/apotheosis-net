using Apotheosis.Core.Features.EmojiCounter.Aggregates;
using Apotheosis.Core.Features.EmojiCounter.Interfaces;
using Apotheosis.Core.Features.EmojiCounter.Models;

namespace Apotheosis.Server.Features.EmojiCounter.Services;

[Scoped]
public class EmojiCounterService(ApotheosisDbContext context) : IEmojiCounterService
{
    public async Task IncrementEmojiCountAsync(ulong guildId, ulong emojiId, string? emojiName)
    {
        var existingEmojiUsage = await context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == guildId && e.EmojiId == emojiId);

        if (existingEmojiUsage != null)
        {
            existingEmojiUsage.Count++;
            await context.SaveChangesAsync();
            return;
        }

        var newEmojiUsage = new EmojiUsage
        {
            Name = emojiName,
            EmojiId = emojiId,
            GuildId = guildId,
            Count = 1,
        };

        await context.EmojiUsages.AddAsync(newEmojiUsage);
        await context.SaveChangesAsync();
    }

    public async Task DecrementEmojiCountAsync(ulong guildId, ulong emojiId)
    {
        var existingEmojiUsage = await context.EmojiUsages.FirstOrDefaultAsync(e => e.GuildId == guildId && e.EmojiId == emojiId);

        if (existingEmojiUsage is { Count: > 0 })
        {
            existingEmojiUsage.Count--;
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<EmojiCounterDto>> GetEmojiCountsForGuildAsync(ulong guildId)
    {
        var emojiUsages = await context.EmojiUsages.Where(emojiUsage => emojiUsage.GuildId == guildId).ToListAsync();

        return emojiUsages.Select(emojiUsage => new EmojiCounterDto
        {
            Name = emojiUsage.Name,
            EmojiId = emojiUsage.EmojiId,
            GuildId = emojiUsage.GuildId,
            Count = emojiUsage.Count
        });
    }
}
