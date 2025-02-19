using Apotheosis.Core.Features.Rank.Exceptions;
using Apotheosis.Core.Features.Rank.Configuration;
using Apotheosis.Core.Features.Rank.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Apotheosis.Core.Features.Rank.Models;

namespace Apotheosis.Core.Features.Rank.Network;
public class LeagueRankClient : ILeagueRankClient
{
    readonly HttpClient _httpClient;
    readonly LeagueRankSettings _leagueRankSettings;
    readonly ILogger<LeagueRankClient> _logger;

    /// <summary>
    /// Gets the number of times to retry requests to League Stats API.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the League Stats API service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public LeagueRankClient(HttpClient httpClient, IOptions<LeagueRankSettings> leagueRankOptions, ILogger<LeagueRankClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _leagueRankSettings = leagueRankOptions.Value;
        _httpClient.BaseAddress = _leagueRankSettings.Url;
    }

    public async Task<IEnumerable<SummonerRankStatsDto>> GetSummonerRankStatsAsync(string summonerName, string tag)
    {
        var response = await _httpClient.GetAsync($"?name={summonerName}&tag={tag}");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error occured retrieving summoner stats: {Data}", data);
            throw new LeagueRankNetworkException("League stats API returned a non-successful status code", null);
        }

        return JsonConvert.DeserializeObject<IEnumerable<SummonerRankStatsDto>>(data) ?? [];
    }
}
