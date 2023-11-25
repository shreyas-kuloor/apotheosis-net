using Apotheosis.Components.AiChat.Models;

namespace Apotheosis.Components.AiChat.Interfaces;

public interface IAiChatService
{
    Task<IEnumerable<AiChatMessageDto>> InitializeThreadToAiAsync(string initialPrompt);
}