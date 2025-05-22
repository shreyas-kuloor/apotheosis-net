using Apotheosis.Core.Features.AiChat.Interfaces;

namespace Apotheosis.Server.Features.AiChat.Services;

[Scoped]
public sealed class AiChatService(IAiChatClient aiChatClient) : IAiChatService
{
    public async Task<string> GetChatResponseAsync(string prompt, CancellationToken cancellationToken)
    {
        return await aiChatClient.GetChatResponseAsync(prompt, cancellationToken);
    }
}