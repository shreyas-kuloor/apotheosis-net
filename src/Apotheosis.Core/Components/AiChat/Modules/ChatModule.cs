using Apotheosis.Core.Components.AiChat.Configuration;
using Apotheosis.Core.Components.AiChat.Exceptions;
using Apotheosis.Core.Components.AiChat.Interfaces;
using Apotheosis.Core.Components.AiChat.Models;
using Apotheosis.Core.Components.DateTime.Interfaces;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Color = NetCord.Color;

namespace Apotheosis.Core.Components.AiChat.Modules;

public sealed class ChatModule(
    IAiChatService aiChatService,
    IAiThreadChannelRepository aiThreadChannelRepository,
    AiChatSettings aiChatSettings,
    IDateTimeService dateTimeService)
    : ApplicationCommandModule<SlashCommandContext>
{
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
                Expiration = dateTimeService.UtcNow.AddMinutes(aiChatSettings.ThreadExpirationMinutes),
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