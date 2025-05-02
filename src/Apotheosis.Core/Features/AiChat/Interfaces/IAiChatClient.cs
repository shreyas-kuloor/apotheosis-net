namespace Apotheosis.Core.Features.AiChat.Interfaces;

public interface IAiChatClient
{
    Task<string> GetChatResponseAsync(string prompt, CancellationToken cancellationToken);
}