using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace Apotheosis.Core.Components.EmojiCounter.Handlers;

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
