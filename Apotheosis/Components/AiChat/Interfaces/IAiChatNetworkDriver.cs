namespace Apotheosis.Components.AiChat.Interfaces;

public interface IAiChatNetworkDriver
{
    Task<TResponse> SendRequestAsync<TResponse>(string path, HttpMethod method, object? request);
}