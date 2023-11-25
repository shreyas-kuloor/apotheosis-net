using Apotheosis.Components.AiChat.Exceptions;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Discord;
using Discord.Interactions;
using Color = Discord.Color;

namespace Apotheosis.Components.AiChat.Modules;

public class ChatModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAiChatService _aiChatService;
    private readonly IAiThreadChannelRepository _aiThreadChannelRepository;

    public ChatModule(
        IAiChatService aiChatService,
        IAiThreadChannelRepository aiThreadChannelRepository)
    {
        _aiChatService = aiChatService;
        _aiThreadChannelRepository = aiThreadChannelRepository;
    }

    [SlashCommand("chat", "Chat with the bot.")]
    public async Task ChatAsync(string prompt)
    {
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

        _aiThreadChannelRepository.StoreThreadChannel(thread.Id);

        var aiChatMessageDtos = response.ToList();
        
        for (var index = 0; index < aiChatMessageDtos.Count; index++)
        {
            await thread.SendMessageAsync(aiChatMessageDtos.FirstOrDefault(r => r.Index == index)!.Content);
        }
        
    }
}