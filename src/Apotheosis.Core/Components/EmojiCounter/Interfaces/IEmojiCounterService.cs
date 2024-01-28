using Apotheosis.Core.Components.EmojiCounter.Models;

namespace Apotheosis.Core.Components.EmojiCounter.Interfaces;

public interface IEmojiCounterService
{
    Task IncrementEmojiCountAsync(ulong guildId, ulong emojiId, string? emojiName);

    Task DecrementEmojiCountAsync(ulong guildId, ulong emojiId);

    Task<IEnumerable<EmojiCounterDto>> GetEmojiCountsForGuildAsync(ulong guildId);
}
