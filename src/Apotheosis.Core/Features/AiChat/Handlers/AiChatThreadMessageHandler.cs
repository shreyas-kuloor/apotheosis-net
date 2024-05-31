using Apotheosis.Core.Features.AiChat.Models;
using Apotheosis.Core.Features.AiChat.Interfaces;
using NetCord.Rest;

namespace Apotheosis.Core.Features.AiChat.Handlers;

[GatewayEvent(nameof(GatewayClient.MessageCreate))]
public sealed class AiChatThreadMessageHandler(
    IAiChatService aiChatService,
    IAiThreadChannelRepository aiThreadChannelRepository)
    : IGatewayEventHandler<Message>
{
    public async ValueTask HandleAsync(Message message)
    {
        if (message is { Channel: GuildThread thread, Author.IsBot: false })
        {
            var activeThreads = aiThreadChannelRepository.GetStoredActiveThreadChannels();

            var activeThread = activeThreads.FirstOrDefault(t => t.ThreadId == thread.Id);
            if (activeThread != null)
            {
                var threadMessages = await thread.GetMessagesAsync().ToListAsync();

                // Convert IMessage to ThreadMessageDto, but ensure the first message, which is always an embed, is read in as a user message.
                var threadContents = threadMessages
                    .OrderBy(m => m.Id)
                    .Where(m => m.InteractionMetadata is null)
                    .Select((m, index) => new MessageDto
                    {
                        Content = index == 0 ? activeThread.InitialMessageContent : m.Content,
                        IsBot = index != 0 && m.Author.IsBot
                    });

                var response = await aiChatService.SendMultipleMessagesToAiAsync(threadContents);

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
    }
}