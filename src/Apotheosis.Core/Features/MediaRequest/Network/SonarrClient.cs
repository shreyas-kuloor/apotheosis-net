using System.Text;
using Apotheosis.Core.Features.MediaRequest.Configuration;
using Apotheosis.Core.Features.MediaRequest.Exceptions;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models.Shared;
using Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Request;
using Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Response;
using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Network;

public class SonarrClient : ISonarrClient
{
    private readonly HttpClient _httpClient;
    private readonly SonarrSettings _sonarrSettings;

    /// <summary>
    /// Gets the number of times to retry requests to Sonarr.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the Sonarr service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public SonarrClient(HttpClient httpClient, IOptions<SonarrSettings> sonarrOptions)
    {
        _httpClient = httpClient;
        _sonarrSettings = sonarrOptions.Value;
        _httpClient.BaseAddress = _sonarrSettings.BaseUrl;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_sonarrSettings.ApiKey}");
    }

    public async Task<IEnumerable<Series>> GetSeriesByTermAsync(string term)
    {
        var response = await _httpClient.GetAsync($"/api/v3/series/lookup?term={term}");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new SonarrNetworkException("Sonarr returned a non-successful status code", null);
        }

        return JsonConvert.DeserializeObject<IEnumerable<Series>>(data) ?? new List<Series>();
    }

    public async Task<IEnumerable<RootFolder>> GetRootFoldersAsync()
    {
        var response = await _httpClient.GetAsync("/api/v3/rootfolder");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new SonarrNetworkException("Sonarr returned a non-successful status code", null);
        }

        return JsonConvert.DeserializeObject<IEnumerable<RootFolder>>(data) ?? new List<RootFolder>();
    }

    public async Task<IEnumerable<QualityProfile>> GetQualityProfilesAsync()
    {
        var response = await _httpClient.GetAsync("/api/v3/qualityprofile");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new SonarrNetworkException("Sonarr returned a non-successful status code", null);
        }

        return JsonConvert.DeserializeObject<IEnumerable<QualityProfile>>(data) ?? new List<QualityProfile>();
    }

    public async Task RequestSeriesAsync(SeriesRequest request)
    {
        var response = await _httpClient.PostAsync(
            "/api/v3/series",
            new StringContent(JsonConvert.SerializeObject(request),
            encoding: Encoding.UTF8,
            "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            throw new SonarrNetworkException("Sonarr returned a non-successful status code", null);
        }
    }
}
