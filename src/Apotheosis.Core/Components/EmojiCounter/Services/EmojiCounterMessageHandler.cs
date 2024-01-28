using System.Text.RegularExpressions;
using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using NetCord.Gateway;

namespace Apotheosis.Core.Components.EmojiCounter.Services;

public sealed partial class EmojiCounterMessageHandler(
    GatewayClient discordClient, 
    IEmojiCounterService emojiCounterService) : IEmojiCounterMessageHandler
{
    [GeneratedRegex(@"<:[^:]+:(\d+)>")]
    private static partial Regex EmojiPatternRegex();

    public void Initialize()
    {
        discordClient.MessageCreate += EmojiMessagedAsync;
    }

    private async ValueTask EmojiMessagedAsync(Message message)
    {
        if (message is { Author.IsBot: false, GuildId: not null })
        {
            var regex = EmojiPatternRegex();

            var successfulMatches = regex.Matches(message.Content).Where(m => m.Success);

            if (successfulMatches.Any())
            {
                var emojiIds = successfulMatches
                    .Select(m => m.Groups[1].Value)
                    .Select(idString =>
                    {
                        _ = ulong.TryParse(idString, out var id);
                        return id;
                    })
                    .Distinct();

                foreach(var emojiId in emojiIds)
                {
                    await emojiCounterService.IncrementEmojiCountAsync(message.GuildId.Value, emojiId);
                }
            }
        }
    }
}
