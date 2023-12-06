using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Apotheosis.Core.Components.AiChat.Configuration;
using Apotheosis.Core.Components.AiChat.Exceptions;
using Apotheosis.Core.Components.AiChat.Interfaces;
using Newtonsoft.Json;

namespace Apotheosis.Core.Components.AiChat.Network;

public sealed class OpenAiNetworkDriver : IAiChatNetworkDriver
{
    private readonly HttpClient _httpClient;
    private readonly AiChatSettings _aiChatSettings;

    /// <summary>
    /// Gets the number of times to retry requests to Imgur.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the Imgur service (in seconds).
    /// </summary>
    public const int Timeout = 10;
    
    public OpenAiNetworkDriver(HttpClient httpClient, AiChatSettings aiChatSettings)
    {
        _httpClient = httpClient;
        _aiChatSettings = aiChatSettings;
        _httpClient.BaseAddress = _aiChatSettings.OpenAiBaseUrl;
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

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _aiChatSettings.OpenAiApiKey);

        using var response = await _httpClient.SendAsync(requestMessage);

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new AiChatNetworkException("OpenAI returned a too many requests error code", AiChatNetworkException.ErrorReason.TokenQuotaReached, null);
            }
            
            throw new AiChatNetworkException("OpenAI returned a non-successful status code", AiChatNetworkException.ErrorReason.Unsuccessful, null);
        }

        var deserializedData = JsonConvert.DeserializeObject<TResponse>(data);

        return deserializedData 
               ?? throw new AiChatNetworkException("An error occured while sending the OpenAI network request", AiChatNetworkException.ErrorReason.Unknown, null);
    }
}