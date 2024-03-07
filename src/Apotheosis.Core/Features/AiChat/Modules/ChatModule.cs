using Apotheosis.Core.Features.AiChat.Configuration;
using Apotheosis.Core.Features.AiChat.Exceptions;
using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Core.Features.AiChat.Models;
using Apotheosis.Core.Features.DateTime.Interfaces;
using NetCord.Rest;
using Color = NetCord.Color;

namespace Apotheosis.Core.Features.AiChat.Modules;

public sealed class ChatModule(
    IAiChatService aiChatService,
    IAiThreadChannelRepository aiThreadChannelRepository,
    IOptions<AiChatSettings> aiChatOptions,
    IDateTimeService dateTimeService)
    : ApplicationCommandModule<SlashCommandContext>
{
    readonly AiChatSettings _aiChatSettings = aiChatOptions.Value;

    [SlashCommand("chat", "Chat with the bot.")]
    public async Task ChatAsync(string prompt)
    {
        if (Context.Channel is GuildThread)
        {
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "This command is not usable in threads. Please call it again in a main channel!",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        await RespondAsync(InteractionCallback.DeferredMessage());

        IEnumerable<AiChatMessageDto> response;
        try
        {
            response = await aiChatService.SendSingleMessageToAiAsync(prompt);
        }
        catch (AiChatNetworkException e)
        {
            if (e.Reason == AiChatNetworkException.ErrorReason.TokenQuotaReached)
            {
                await FollowupAsync(new InteractionMessageProperties
                {
                    Content = "The ChatGPT token quota has been reached. Please ask an admin to check their usage limits or billing details.",
                    Flags = MessageFlags.Ephemeral
                });
                return;
            }

            await FollowupAsync(new InteractionMessageProperties
            {
                Content = "Sorry, an error occurred while trying to reach ChatGPT. Please try again later or contact an admin.",
                Flags = MessageFlags.Ephemeral
            });
            return;
        }

        var textChannel = Context.Channel;
        var embed = new EmbedProperties
        {
            Author = new EmbedAuthorProperties
            {
                Name = Context.User.GlobalName,
                IconUrl = Context.User.GetAvatarUrl().ToString()
            },
            Description = prompt,
            Color = new Color(255, 128, 0),
        };

        var embedFollowupMessage = await FollowupAsync(new InteractionMessageProperties
        {
            Embeds = new List<EmbedProperties>
            {
                embed
            }
        });

        var thread = await Context.Client.Rest.CreateGuildThreadAsync(
            textChannel.Id,
            embedFollowupMessage.Id,
            new GuildThreadProperties($"Chatting with {Context.User.GlobalName}")
            {
                AutoArchiveDuration = 1440,
                ChannelType = ChannelType.PublicGuildThread
            });

        aiThreadChannelRepository.ClearExpiredThreadChannels();

        aiThreadChannelRepository.StoreThreadChannel(
            new ThreadChannelDto
            {
                ThreadId = thread.Id,
                Expiration = dateTimeService.UtcNow.AddMinutes(_aiChatSettings.ThreadExpirationMinutes),
                InitialMessageContent = prompt
            });

        var aiChatMessageDtos = response.ToList();

        for (var index = 0; index < aiChatMessageDtos.Count; index++)
        {
            await thread.SendMessageAsync(new MessageProperties
            {
                Content = aiChatMessageDtos.FirstOrDefault(r => r.Index == index)!.Content
            });
        }

    }
}