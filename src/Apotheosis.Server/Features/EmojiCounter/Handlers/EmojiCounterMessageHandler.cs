using Apotheosis.Core.Features.EmojiCounter.Interfaces;

namespace Apotheosis.Server.Features.EmojiCounter.Handlers;

[GatewayEvent(nameof(GatewayClient.MessageCreate))]
public sealed partial class EmojiCounterMessageHandler(
    IEmojiCounterService emojiCounterService) 
    : IGatewayEventHandler<Message>
{
    [GeneratedRegex(@"<:([^:]+):(\d+)>")]
    private static partial Regex EmojiPatternRegex();

    public async ValueTask HandleAsync(Message message)
    {
        if (message is { Author.IsBot: false, GuildId: not null })
        {
            var regex = EmojiPatternRegex();

            var successfulMatches = regex.Matches(message.Content).Where(m => m.Success);

            if (successfulMatches.Any())
            {
                var emojiIds = successfulMatches
                    .Select(m => 
                    {
                        var name = m.Groups[1].Value;
                        var id = ulong.TryParse(m.Groups[2].Value, out ulong parsedId) ? parsedId : 0;
                        return (Name: name, Id: id);
                    })
                    .Where(e => e.Id != 0)
                    .Distinct();

                foreach(var (emojiName, emojiId) in emojiIds)
                {
                    await emojiCounterService.IncrementEmojiCountAsync(message.GuildId.Value, emojiId, emojiName);
                }
            }
        }
    }
}
