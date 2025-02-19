using Microsoft.Extensions.Logging;
using Apotheosis.Core.Features.Stats.Configuration;
using Apotheosis.Core.Features.Stats.Interfaces;
using Apotheosis.Core.Features.Stats.Exceptions;

namespace Apotheosis.Core.Features.Stats.Network;
public class LeagueStatsClient : ILeagueStatsClient
{
    readonly HttpClient _httpClient;
    readonly LeagueStatsSettings _leagueStatsSettings;
    readonly ILogger<LeagueStatsClient> _logger;

    /// <summary>
    /// Gets the number of times to retry requests to League Stats API.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the League Stats API service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public LeagueStatsClient(HttpClient httpClient, IOptions<LeagueStatsSettings> leagueStatsOptions, ILogger<LeagueStatsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _leagueStatsSettings = leagueStatsOptions.Value;
        _httpClient.BaseAddress = _leagueStatsSettings.BaseUrl;
    }

    public async Task<string> GetSummonerStatsAsync(string summonerName, string tag)
    {
        var response = await _httpClient.GetAsync($"/stats?name={summonerName}&tag={tag}");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error occured retrieving summoner stats: {Data}", data);
            throw new LeagueStatsNetworkException("League stats API returned a non-successful status code", null);
        }

        return data;
    }
}
