using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Server.Features.AiChat.Configuration;
using Apotheosis.Server.Features.AiChat.Exceptions;
using Microsoft.Extensions.Logging;

namespace Apotheosis.Server.Features.AiChat.Modules;

[GatewayEvent(nameof(GatewayClient.MessageCreate))]
public sealed class ChatMessageHandler(
    IAiChatService aiChatService,
    ILogger<ChatMessageHandler> logger,
    IOptions<AiChatSettings> aiChatOptions)
    : IGatewayEventHandler<Message>
{
    readonly AiChatSettings _aiChatSettings = aiChatOptions.Value;
    
    public async ValueTask HandleAsync(Message message)
    {
        if (message.MentionedUsers.Any(mu => mu.Id == _aiChatSettings.TargetId))
        {
            logger.LogInformation("{User} used @bot {Content}", message.Author.GlobalName, message.Content);

            var reply = await message.ReplyAsync(new ReplyMessageProperties { Content = "Thinking..." });
            
            string response;
            try
            {
                response = await aiChatService.GetChatResponseAsync(message.Content, CancellationToken.None);
            }
            catch (AiChatNetworkException)
            {
                await reply.ModifyAsync(existingMsg =>
                {
                    existingMsg.Content =
                        "Sorry, an error occurred while trying to reach the AI service. Please try again later or contact an admin.";
                });
                return;
            }
        
            if (string.IsNullOrEmpty(response))
            {
                await reply.ModifyAsync(existingMsg =>
                {
                    existingMsg.Content =
                        "Sorry, an error occurred while trying to reach the AI service. Please try again later or contact an admin.";
                });
                return;
            }
            
            await reply.ModifyAsync(existingMsg =>
            {
                existingMsg.Content = response;
            });
        }
    }
}