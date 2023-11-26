using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Discord;
using Discord.WebSocket;

namespace Apotheosis.Components.AiChat.Services;

public sealed class AiChatThreadMessageHandler : IAiChatThreadMessageHandler
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IAiChatService _aiChatService;
    private readonly IAiThreadChannelRepository _aiThreadChannelRepository;

    public AiChatThreadMessageHandler(
        DiscordSocketClient discordSocketClient,
        IAiChatService aiChatService,
        IAiThreadChannelRepository aiThreadChannelRepository)
    {
        _discordSocketClient = discordSocketClient;
        _aiChatService = aiChatService;
        _aiThreadChannelRepository = aiThreadChannelRepository;
    }

    public void InitializeAsync()
    {
        _discordSocketClient.MessageReceived += MessageReceivedAsync;
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Channel is IThreadChannel thread && !message.Author.IsBot)
        {
            var activeThreads = _aiThreadChannelRepository.GetStoredActiveThreadChannels();

            var activeThread = activeThreads.FirstOrDefault(t => t.ThreadId == thread.Id);
            if (activeThread != null)
            {
                var threadMessages = await thread.GetMessagesAsync(limit: 50).FlattenAsync();

                // Convert IMessage to ThreadMessageDto, but ensure the first message, which is always an embed, is read in as a user message.
                var threadContents = threadMessages
                    .OrderBy(m => m.Id)
                    .Where(m => m.Interaction is null)
                    .Select((m, index) => new ThreadMessageDto
                    {
                        Content = index == 0 ? activeThread.InitialMessageContent : m.Content,
                        IsBot = index != 0 && m.Author.IsBot
                    });

                var response = await _aiChatService.SendThreadToAiAsync(threadContents);
                
                var aiChatMessageDtos = response.ToList();
        
                for (var index = 0; index < aiChatMessageDtos.Count; index++)
                {
                    await thread.SendMessageAsync(aiChatMessageDtos.FirstOrDefault(r => r.Index == index)!.Content);
                }
            }
        }
    }
}