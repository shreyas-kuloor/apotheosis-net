using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Color = NetCord.Color;

namespace Apotheosis.Core.Components.EmojiCounter.Modules;

public sealed class EmojiCountModule(IEmojiCounterService emojiCounterService) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("emoji-count", "Get the reaction and usage history of emojis in this guild.")]
    public async Task EmojiCountAsync()
    {
        if (Context.Guild is not null)
        {
            await RespondAsync(InteractionCallback.DeferredMessage());

            var emojiCounts = await emojiCounterService.GetEmojiCountsForGuildAsync(Context.Guild.Id);
            var currentUser = await Context.Client.Rest.GetCurrentUserAsync();

            var textChannel = Context.Channel;
            var embed = new EmbedProperties
            {
                Author = new EmbedAuthorProperties
                {
                    Name = currentUser.GlobalName,
                    IconUrl = currentUser.GetAvatarUrl().ToString()
                },
                Description = "Emoji usage statistics for this guild.",
                Color = new Color(128, 255, 0),
                Fields = emojiCounts
                    .Where(ec => ec.GuildId == Context.Guild.Id)
                    .Select(ec => new EmbedFieldProperties { Name = $"<:{ec.Name}:{ec.EmojiId}>", Value = $"{ec.Count}" })
            };
        }
        
    }
}
