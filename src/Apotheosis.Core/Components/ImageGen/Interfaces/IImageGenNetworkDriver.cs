namespace Apotheosis.Core.Components.ImageGen.Interfaces;

public interface IImageGenNetworkDriver
{
    Task<TResponse> SendRequestAsync<TResponse>(string path, HttpMethod method, object? request);
}