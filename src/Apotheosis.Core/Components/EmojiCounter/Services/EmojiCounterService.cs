using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using Apotheosis.Infrastructure.Data;
using Apotheosis.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apotheosis.Core.Components.EmojiCounter.Services;

public class EmojiCounterService(ApotheosisDbContext context) : IEmojiCounterService
{
    public async Task IncrementEmojiCountAsync(ulong guildId, ulong emojiId)
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

        if (existingEmojiUsage != null)
        {
            if (existingEmojiUsage.Count > 0)
            {
                existingEmojiUsage.Count--;
                await context.SaveChangesAsync();
            }           
        }
    }
}
