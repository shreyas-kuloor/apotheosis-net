using Apotheosis.Core.Features.EmojiCounter.Interfaces;

namespace Apotheosis.Core.Features.EmojiCounter.Handlers;

[GatewayEvent(nameof(GatewayClient.MessageReactionRemove))]
public sealed class EmojiCounterReactionRemoveHandler(
    IEmojiCounterService emojiCounterService)
    : IGatewayEventHandler<MessageReactionRemoveEventArgs>
{
    public async ValueTask HandleAsync(MessageReactionRemoveEventArgs args)
    {
        if (args.GuildId != null && !args.Emoji.IsStandard)
        {
            await emojiCounterService.DecrementEmojiCountAsync(args.GuildId.Value, args.Emoji.Id);
        }
    }
}
