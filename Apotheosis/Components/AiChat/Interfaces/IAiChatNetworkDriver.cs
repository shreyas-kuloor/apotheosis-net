namespace Apotheosis.Components.AiChat.Interfaces;

public interface IAiChatNetworkDriver
{
    Task<TResponse?> SendRequestAsync<TRequest, TResponse>(string path, HttpMethod method, TRequest? request);
}
