using Apotheosis.Core.Features.AiChat.Models;

namespace Apotheosis.Core.Features.AiChat.Interfaces;

public interface IAiChatService
{
    Task<IEnumerable<AiChatMessageDto>> SendSingleMessageToAiAsync(string prompt, string? systemMessage = null);

    Task<IEnumerable<AiChatMessageDto>> SendMultipleMessagesToAiAsync(IEnumerable<MessageDto> messages);
}