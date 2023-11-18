using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Apotheosis.Components.ImageUpload.Configuration;
using Apotheosis.Components.ImageUpload.Exceptions;
using Apotheosis.Components.ImageUpload.Interfaces;
using Apotheosis.Components.ImageUpload.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Apotheosis.Components.ImageUpload.Network;

public sealed class ImgurNetworkDriver : IImageUploadNetworkDriver
{
    private readonly HttpClient _httpClient;
    private readonly ImageUploadSettings _imageUploadSettings;

    /// <summary>
    /// Gets the number of times to retry requests to Imgur.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the Imgur service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public ImgurNetworkDriver(HttpClient httpClient, IOptions<ImageUploadSettings> imageUploadOptions)
    {
        _httpClient = httpClient;
        _imageUploadSettings = imageUploadOptions.Value;
        _httpClient.BaseAddress = _imageUploadSettings.BaseUrl;
    }

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(string path, HttpMethod method, TRequest? request)
    {
        using var requestMessage = new HttpRequestMessage(method, path);

        if (request != null)
        {
            var serialized = JsonConvert.SerializeObject(request);
            var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            requestMessage.Content = content;
        }
        
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

        using var response = await _httpClient.SendAsync(requestMessage);

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new ImageUploadNetworkException(null, null);
        }

        var deserializedData = JsonConvert.DeserializeObject<TResponse>(data);

        return deserializedData 
               ?? throw new ImageUploadNetworkException("An error occured while fetching the imgur access token.");
    }

    private async Task<string> GetAccessTokenAsync()
    {
        using var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/oauth2/token");

        var tokenRequest = new ImageUploadAccessTokenRequest
        {
            RefreshToken = _imageUploadSettings.RefreshToken,
            GrantType = "refresh_token",
            ClientId = _imageUploadSettings.ClientId,
            ClientSecret = _imageUploadSettings.ClientSecret,
        };
        
        tokenRequestMessage.Content = new StringContent(
            JsonConvert.SerializeObject(tokenRequest), 
            Encoding.UTF8, 
            MediaTypeNames.Application.Json);

        using var tokenResponse = await _httpClient.SendAsync(tokenRequestMessage);

        var tokenData = await tokenResponse.Content.ReadAsStringAsync();

        var deserializedTokenData = JsonConvert.DeserializeObject<ImageUploadAccessTokenResponse>(tokenData);

        return deserializedTokenData?.AccessToken 
               ?? throw new ImageUploadNetworkException("An error occured while fetching the imgur access token.");

    }
}