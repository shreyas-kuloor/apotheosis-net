using System.Net.Mime;
using System.Text;
using Apotheosis.Core.Features.TextToSpeech.Interfaces;
using Apotheosis.Server.Features.TextToSpeech.Configuration;
using Apotheosis.Server.Features.TextToSpeech.Exceptions;
using Newtonsoft.Json;

namespace Apotheosis.Server.Features.TextToSpeech.Network;

public sealed class ElevenLabsNetworkDriver : ITextToSpeechNetworkDriver
{
    readonly HttpClient _httpClient;
    readonly TextToSpeechSettings _textToSpeechSettings;

    /// <summary>
    /// Gets the number of times to retry requests to Imgur.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the Imgur service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public ElevenLabsNetworkDriver(HttpClient httpClient, IOptions<TextToSpeechSettings> textToSpeechOptions)
    {
        _httpClient = httpClient;
        _textToSpeechSettings = textToSpeechOptions.Value;
        _httpClient.BaseAddress = _textToSpeechSettings.ElevenLabsBaseUrl;
    }

    public async Task<TResponse> SendRequestAsync<TResponse>(string path, HttpMethod method, object? request)
    {
        using var requestMessage = new HttpRequestMessage(method, path);

        if (request != null)
        {
            var serialized = JsonConvert.SerializeObject(request);
            var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            requestMessage.Content = content;
        }

        requestMessage.Headers.Add("xi-api-key", _textToSpeechSettings.ElevenLabsApiKey);

        using var response = await _httpClient.SendAsync(requestMessage);

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new TextToSpeechNetworkException("Eleven Labs returned a non-successful status code", null);
        }

        var deserializedData = JsonConvert.DeserializeObject<TResponse>(data);

        return deserializedData
               ?? throw new TextToSpeechNetworkException("An error occured while sending the Eleven Labs network request.");
    }

    public async Task<Stream> SendRequestReceiveStreamAsync(string path, HttpMethod method, object? request)
    {
        using var requestMessage = new HttpRequestMessage(method, path);

        if (request != null)
        {
            var serialized = JsonConvert.SerializeObject(request);
            var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            requestMessage.Content = content;
        }

        requestMessage.Headers.Add("xi-api-key", _textToSpeechSettings.ElevenLabsApiKey);

        var response = await _httpClient.SendAsync(requestMessage);

        var data = await response.Content.ReadAsStreamAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new TextToSpeechNetworkException("Eleven Labs returned a non-successful status code", null);
        }

        return data;
    }
}