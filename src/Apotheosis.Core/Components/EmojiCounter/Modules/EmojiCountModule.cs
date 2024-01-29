﻿using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Color = NetCord.Color;

namespace Apotheosis.Core.Components.EmojiCounter.Modules;

public sealed class EmojiCountModule(IEmojiCounterService emojiCounterService) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("emojicount", "Get the reaction and usage count of emojis in this guild.")]
    public async Task EmojiCountAsync()
    {
        if (Context.Guild is not null)
        {
            await RespondAsync(InteractionCallback.DeferredMessage(NetCord.MessageFlags.Ephemeral));

            var emojiCounts = await emojiCounterService.GetEmojiCountsForGuildAsync(Context.Guild.Id);

            var embed = new EmbedProperties
            {
                Title = $"Emoji counts for {Context.Guild.Name}",
                Color = new Color(73, 52, 235),
                Fields = emojiCounts
                    .Where(ec => ec.GuildId == Context.Guild.Id)
                    .Select(ec => new EmbedFieldProperties { Name = $"<:{ec.Name}:{ec.EmojiId}>", Value = $"{ec.Count}", Inline = false })
            };

            await FollowupAsync(new InteractionMessageProperties
            {
                Embeds = new List<EmbedProperties>
                {
                    embed
                },
                Flags = NetCord.MessageFlags.Ephemeral
            });
        }
        
    }
}
