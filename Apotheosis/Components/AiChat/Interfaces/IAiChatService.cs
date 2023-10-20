namespace Apotheosis.Components.AiChat.Interfaces;

public interface IAiChatService
{
    Task<string> InitiateChatAndGetResponseAsync(string message);
}