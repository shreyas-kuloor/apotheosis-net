namespace Apotheosis.Components.ImageUpload.Interfaces;

public interface IImageUploadNetworkDriver
{
    Task<TResponse> SendRequestAsync<TRequest, TResponse>(string path, HttpMethod method, TRequest? request);
}