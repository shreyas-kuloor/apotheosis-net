namespace Apotheosis.Core.Components.EmojiCounter.Interfaces;

public interface IEmojiCounterService
{
    Task IncrementEmojiCountAsync(ulong guildId, ulong emojiId);

    Task DecrementEmojiCountAsync(ulong guildId, ulong emojiId);
}
