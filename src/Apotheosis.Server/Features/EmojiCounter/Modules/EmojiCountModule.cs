using Apotheosis.Core.Features.EmojiCounter.Interfaces;
using Microsoft.Extensions.Logging;
using Color = NetCord.Color;

namespace Apotheosis.Server.Features.EmojiCounter.Modules;

public sealed class EmojiCountModule(
    IEmojiCounterService emojiCounterService, 
    ILogger<EmojiCountModule> logger) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("emojicount", "Get the reaction and usage count of emojis in this guild.")]
    public async Task EmojiCountAsync()
    {
        if (Context.Guild is not null)
        {
            logger.LogInformation("{User} used /emojicount", Context.User.GlobalName);
            await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

            var emojiCounts = await emojiCounterService.GetEmojiCountsForGuildAsync(Context.Guild.Id);
            var emojiStrings = emojiCounts
                .Where(ec => ec.GuildId == Context.Guild.Id)
                .OrderByDescending(ec => ec.Count)
                .Take(15)
                .Select(ec => $"<:{ec.Name}:{ec.EmojiId}> ({ec.Count})");

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
