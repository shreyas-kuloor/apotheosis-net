﻿using Apotheosis.Core.Features.EmojiCounter.Interfaces;

namespace Apotheosis.Core.Features.EmojiCounter.Handlers;

[GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
public sealed class EmojiCounterReactionAddHandler(
    IEmojiCounterService emojiCounterService)
    : IGatewayEventHandler<MessageReactionAddEventArgs>
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        if (args.GuildId != null && args.Emoji.Id.HasValue)
        {
            await emojiCounterService.IncrementEmojiCountAsync(args.GuildId.Value, args.Emoji.Id.Value, args.Emoji.Name);
        }
    }
}
