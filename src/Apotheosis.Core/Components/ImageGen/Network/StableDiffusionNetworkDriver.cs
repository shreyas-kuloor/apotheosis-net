using System.Net.Mime;
using System.Text;
using Apotheosis.Core.Components.ImageGen.Configuration;
using Apotheosis.Core.Components.ImageGen.Exceptions;
using Apotheosis.Core.Components.ImageGen.Interfaces;
using Newtonsoft.Json;

namespace Apotheosis.Core.Components.ImageGen.Network;

public sealed class StableDiffusionNetworkDriver : IImageGenNetworkDriver
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Gets the number of times to retry requests to Imgur.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the Imgur service (in seconds).
    /// </summary>
    public const int Timeout = 10;
    
    public StableDiffusionNetworkDriver(HttpClient httpClient, ImageGenSettings imageGenSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = imageGenSettings.StableDiffusionBaseUrl;
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

        using var response = await _httpClient.SendAsync(requestMessage);

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new ImageGenNetworkException("Stable Diffusion returned a non-successful status code", null);
        }

        var deserializedData = JsonConvert.DeserializeObject<TResponse>(data);

        return deserializedData 
               ?? throw new ImageGenNetworkException("An error occured while sending the Stable Diffusion network request.");
    }
}