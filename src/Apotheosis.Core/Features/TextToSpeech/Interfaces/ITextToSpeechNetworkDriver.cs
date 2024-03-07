namespace Apotheosis.Core.Features.TextToSpeech.Interfaces;

public interface ITextToSpeechNetworkDriver
{
    Task<TResponse> SendRequestAsync<TResponse>(string path, HttpMethod method, object? request);

    Task<Stream> SendRequestReceiveStreamAsync(string path, HttpMethod method, object? request);
}