using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Exceptions;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Apotheosis.Components.DateTime.Interfaces;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Options;
using Color = Discord.Color;

namespace Apotheosis.Components.AiChat.Modules;

public sealed class ChatModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAiChatService _aiChatService;
    private readonly IAiThreadChannelRepository _aiThreadChannelRepository;
    private readonly AiChatSettings _aiChatSettings;
    private readonly IDateTimeService _dateTimeService;

    public ChatModule(
        IAiChatService aiChatService,
        IAiThreadChannelRepository aiThreadChannelRepository,
        IOptions<AiChatSettings> aiChatOptions,
        IDateTimeService dateTimeService)
    {
        _aiChatService = aiChatService;
        _aiThreadChannelRepository = aiThreadChannelRepository;
        _aiChatSettings = aiChatOptions.Value;
        _dateTimeService = dateTimeService;
    }

    [SlashCommand("chat", "Chat with the bot.")]
    public async Task ChatAsync(string prompt)
    {
        if (Context.Channel is IThreadChannel)
        {
            await RespondAsync("This command is not usable in threads. Please call it again in a main channel!");
            return;
        }
        
        await DeferAsync();

        IEnumerable<AiChatMessageDto> response;
        try
        {
            response = await _aiChatService.InitializeThreadToAiAsync(prompt);
        }
        catch (AiChatNetworkException e)
        {
            if (e.Reason == AiChatNetworkException.ErrorReason.TokenQuotaReached)
            {
                await FollowupAsync("The ChatGPT token quota has been reached. Please ask an admin to check their usage limits or billing details.");
                return;
            }

            await FollowupAsync("Sorry, an error occurred while trying to reach ChatGPT. Please try again later or contact an admin.");
            return;
        }
        
        var textChannel = (Context.Channel as ITextChannel)!;
        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder().WithName(Context.User.GlobalName).WithIconUrl(Context.User.GetAvatarUrl()),
            Description = prompt,
            Color = Color.Gold
        }.Build();
        
        await FollowupAsync(embed: embed);
        var initialMessage = (await Context.Interaction.GetOriginalResponseAsync() as IMessage)!;

        var thread = await textChannel.CreateThreadAsync(
            $"Chatting with {Context.User.GlobalName}",
            ThreadType.PublicThread,
            ThreadArchiveDuration.OneDay,
            initialMessage);

        _aiThreadChannelRepository.ClearExpiredThreadChannels();
        
        _aiThreadChannelRepository.StoreThreadChannel(
            new ThreadChannelDto
            {
                ThreadId = thread.Id, 
                Expiration = _dateTimeService.UtcNow.AddMinutes(_aiChatSettings.ThreadExpirationMinutes),
                InitialMessageContent = prompt
            });

        var aiChatMessageDtos = response.ToList();
        
        for (var index = 0; index < aiChatMessageDtos.Count; index++)
        {
            await thread.SendMessageAsync(aiChatMessageDtos.FirstOrDefault(r => r.Index == index)!.Content);
        }
        
    }
}