using Apotheosis.Core.Features.EmojiCounter.Interfaces;

namespace Apotheosis.Server.Features.EmojiCounter.Handlers;

[GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
public sealed class EmojiCounterReactionAddHandler(
    IEmojiCounterService emojiCounterService)
    : IGatewayEventHandler<MessageReactionAddEventArgs>
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        if (args is { GuildId: not null, Emoji.Id: not null })
        {
            await emojiCounterService.IncrementEmojiCountAsync(args.GuildId.Value, args.Emoji.Id.Value, args.Emoji.Name);
        }
    }
}
