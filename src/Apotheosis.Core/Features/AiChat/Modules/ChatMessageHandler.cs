using Apotheosis.Core.Features.AiChat.Configuration;
using Apotheosis.Core.Features.AiChat.Exceptions;
using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Core.Features.Logging.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Rest;

namespace Apotheosis.Core.Features.AiChat.Modules;

[GatewayEvent(nameof(GatewayClient.MessageCreate))]
public sealed class ChatMessageHandler(
    IAiChatService aiChatService,
    ILogService<ChatMessageHandler> logger,
    IOptions<AiChatSettings> aiChatOptions)
    : IGatewayEventHandler<Message>
{
    readonly AiChatSettings _aiChatSettings = aiChatOptions.Value;
    
    public async ValueTask HandleAsync(Message message)
    {
        if (message.MentionedUsers.Values.Any(mu => mu.Id == _aiChatSettings.TargetId))
        {
            logger.Log(LogLevel.Information, null, $"{message.Author.GlobalName} used @bot {message.Content}");

            var reply = await message.ReplyAsync(new ReplyMessageProperties { Flags = MessageFlags.Loading });
            
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
                    existingMsg.Flags = null;
                });
                return;
            }
        
            if (string.IsNullOrEmpty(response))
            {
                await reply.ModifyAsync(existingMsg =>
                {
                    existingMsg.Content =
                        "Sorry, an error occurred while trying to reach the AI service. Please try again later or contact an admin.";
                    existingMsg.Flags = null;
                });
                return;
            }
            
            await reply.ModifyAsync(existingMsg =>
            {
                existingMsg.Content = response;
                existingMsg.Flags = null;
            });
        }
    }
}