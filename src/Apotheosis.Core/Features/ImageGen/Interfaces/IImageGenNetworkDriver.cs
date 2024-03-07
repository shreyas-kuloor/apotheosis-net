namespace Apotheosis.Core.Features.ImageGen.Interfaces;

public interface IImageGenNetworkDriver
{
    Task<TResponse> SendRequestAsync<TResponse>(string path, HttpMethod method, object? request);
}