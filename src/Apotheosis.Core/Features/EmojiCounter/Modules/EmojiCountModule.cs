using Apotheosis.Core.Features.EmojiCounter.Interfaces;
using Apotheosis.Core.Features.Logging.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Rest;
using Color = NetCord.Color;

namespace Apotheosis.Core.Features.EmojiCounter.Modules;

public sealed class EmojiCountModule(
    IEmojiCounterService emojiCounterService, 
    ILogService<EmojiCountModule> logger) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("emojicount", "Get the reaction and usage count of emojis in this guild.")]
    public async Task EmojiCountAsync()
    {
        if (Context.Guild is not null)
        {
            logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /emojicount");
            await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

            var emojiCounts = await emojiCounterService.GetEmojiCountsForGuildAsync(Context.Guild.Id);
            var emojiStrings = emojiCounts.Where(ec => ec.GuildId == Context.Guild.Id).Select(ec => $"<:{ec.Name}:{ec.EmojiId}> ({ec.Count})");

            var embed = new EmbedProperties
            {
                Title = $"Emoji counts for {Context.Guild.Name}",
                Color = new Color(73, 52, 235),
                Description = string.Join(",", emojiStrings)
            };

            await FollowupAsync(new InteractionMessageProperties
            {
                Embeds = new List<EmbedProperties>
                {
                    embed
                },
                Flags = MessageFlags.Ephemeral
            });
        }

    }
}
