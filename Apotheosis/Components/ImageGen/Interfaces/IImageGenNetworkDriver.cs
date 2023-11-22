namespace Apotheosis.Components.ImageGen.Interfaces;

public interface IImageGenNetworkDriver
{
    Task<TResponse> SendRequestAsync<TResponse>(string path, HttpMethod method, object? request);
}