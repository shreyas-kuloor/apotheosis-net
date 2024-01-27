using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using NetCord.Gateway;

namespace Apotheosis.Core.Components.EmojiCounter.Services;

public sealed class EmojiCounterReactionHandler(GatewayClient discordClient, IEmojiCounterService emojiCounterService) : IEmojiCounterReactionHandler
{
    public void Initialize()
    {
        discordClient.MessageReactionAdd += ReactionAddedAsync;
        discordClient.MessageReactionRemove += ReactionRemovedAsync;
    }

    private async ValueTask ReactionAddedAsync(MessageReactionAddEventArgs args)
    {
        if (args.GuildId != null)
        {
            await emojiCounterService.IncrementEmojiCountAsync(args.GuildId.Value, args.Emoji.Id);
        }
    }

    private async ValueTask ReactionRemovedAsync(MessageReactionRemoveEventArgs args)
    {
        if (args.GuildId != null)
        {
            await emojiCounterService.DecrementEmojiCountAsync(args.GuildId.Value, args.Emoji.Id);
        }
    }
}
