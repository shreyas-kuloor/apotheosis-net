using System.Text;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models.Radarr.Request;
using Apotheosis.Core.Features.MediaRequest.Models.Radarr.Response;
using Apotheosis.Core.Features.MediaRequest.Models.Shared;
using Apotheosis.Server.Features.MediaRequest.Configuration;
using Apotheosis.Server.Features.MediaRequest.Exceptions;
using Newtonsoft.Json;

namespace Apotheosis.Server.Features.MediaRequest.Network;

public class RadarrClient : IRadarrClient
{
    readonly HttpClient _httpClient;
    readonly RadarrSettings _radarrSettings;

    /// <summary>
    /// Gets the number of times to retry requests to Radarr.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the Radarr service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public RadarrClient(HttpClient httpClient, IOptions<RadarrSettings> radarrOptions)
    {
        _httpClient = httpClient;
        _radarrSettings = radarrOptions.Value;
        _httpClient.BaseAddress = _radarrSettings.BaseUrl;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_radarrSettings.ApiKey}");
    }

    public async Task<IEnumerable<Movie>> GetMovieByTermAsync(string term)
    {
        var response = await _httpClient.GetAsync($"/api/v3/movie/lookup?term={term}");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new RadarrNetworkException("Radarr returned a non-successful status code", null);
        }

        return JsonConvert.DeserializeObject<IEnumerable<Movie>>(data) ?? new List<Movie>();
    }

    public async Task<IEnumerable<RootFolder>> GetRootFoldersAsync()
    {
        var response = await _httpClient.GetAsync("/api/v3/rootfolder");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new RadarrNetworkException("Radarr returned a non-successful status code", null);
        }

        return JsonConvert.DeserializeObject<IEnumerable<RootFolder>>(data) ?? new List<RootFolder>();
    }

    public async Task<IEnumerable<QualityProfile>> GetQualityProfilesAsync()
    {
        var response = await _httpClient.GetAsync("/api/v3/qualityprofile");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new RadarrNetworkException("Radarr returned a non-successful status code", null);
        }

        return JsonConvert.DeserializeObject<IEnumerable<QualityProfile>>(data) ?? new List<QualityProfile>();
    }

    public async Task RequestMovieAsync(MovieRequest request)
    {
        var response = await _httpClient.PostAsync(
            "/api/v3/movie", 
            new StringContent(JsonConvert.SerializeObject(request), 
            encoding: Encoding.UTF8, 
            "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            throw new RadarrNetworkException("Radarr returned a non-successful status code", null);
        }
    }
}
