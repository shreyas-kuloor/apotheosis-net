using Apotheosis.Components.AiChat.Models;

namespace Apotheosis.Components.AiChat.Interfaces;

public interface IAiChatService
{
    Task<IEnumerable<AiChatMessageDto>> SendSingleMessageToAiAsync(string prompt, string? systemMessage = null);
    
    Task<IEnumerable<AiChatMessageDto>> SendMultipleMessagesToAiAsync(IEnumerable<MessageDto> messages);
}