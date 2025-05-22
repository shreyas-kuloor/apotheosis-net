using Apotheosis.Core.Features.Recap.Interfaces;
using Apotheosis.Server.Features.Recap.Configuration;
using Apotheosis.Server.Features.Recap.Exceptions;
using Microsoft.Extensions.Logging;

namespace Apotheosis.Server.Features.Recap.Network;
public class LeagueRecapClient : ILeagueRecapClient
{
    readonly HttpClient _httpClient;
    readonly LeagueRecapSettings _leagueRecapSettings;
    readonly ILogger<LeagueRecapClient> _logger;

    /// <summary>
    /// Gets the number of times to retry requests to League Stats API.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the League Stats API service (in seconds).
    /// </summary>
    public const int Timeout = 120;

    public LeagueRecapClient(HttpClient httpClient, IOptions<LeagueRecapSettings> leagueRecapOptions, ILogger<LeagueRecapClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _leagueRecapSettings = leagueRecapOptions.Value;
        _httpClient.BaseAddress = _leagueRecapSettings.Url;
    }

    public async Task<string> GetSummonerRecapAsync(string summonerName, string tag)
    {
        var response = await _httpClient.GetAsync($"?name={summonerName}&tag={tag}");

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error occured retrieving summoner recap: {Data}", data);
            throw new LeagueRecapNetworkException("League recap API returned a non-successful status code", null);
        }

        return data;
    }
}
