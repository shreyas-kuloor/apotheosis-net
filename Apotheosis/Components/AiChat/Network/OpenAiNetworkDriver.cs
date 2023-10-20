using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Exceptions;
using Apotheosis.Components.AiChat.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Apotheosis.Components.AiChat.Network;

public sealed class OpenAiNetworkDriver : IAiChatNetworkDriver
{
    private readonly HttpClient _httpClient;
    private readonly AiChatSettings _aiChatSettings;

    /// <summary>
    /// Gets the number of times to retry requests to OpenAI.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the OpenAI service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public OpenAiNetworkDriver(HttpClient httpClient, IOptions<AiChatSettings> aiChatOptions)
    {
        _httpClient = httpClient;
        _aiChatSettings = aiChatOptions.Value;
        _httpClient.BaseAddress = _aiChatSettings.OpenAiBaseUrl;
    }

    public async Task<TResponse?> SendRequestAsync<TRequest, TResponse>(string path, HttpMethod method, TRequest? request)
    {
        using var requestMessage = new HttpRequestMessage();
        requestMessage.Method = method;
        requestMessage.RequestUri = new Uri(path);

        if (request != null)
        {
            var serialized = JsonConvert.SerializeObject(request);
            var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            requestMessage.Content = content;
        }

        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _aiChatSettings.OpenAiApiKey);

        using var response = await _httpClient.SendAsync(new HttpRequestMessage(method, path));

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new AiChatNetworkException(AiChatNetworkException.ErrorReason.TokenQuotaReached, null, null);
            }

            throw new AiChatNetworkException(AiChatNetworkException.ErrorReason.UnexpectedResponse, null, null);
        }

        var deserialized = JsonConvert.DeserializeObject<TResponse>(data);

        return deserialized;
    }
}
