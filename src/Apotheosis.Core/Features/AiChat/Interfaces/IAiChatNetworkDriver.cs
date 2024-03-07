namespace Apotheosis.Core.Features.AiChat.Interfaces;

public interface IAiChatNetworkDriver
{
    Task<TResponse> SendRequestAsync<TResponse>(string path, HttpMethod method, object? request);
}