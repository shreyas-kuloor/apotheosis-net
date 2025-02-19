using Apotheosis.Core.Features.Stats.Interfaces;

namespace Apotheosis.Core.Features.Stats.Services;

[Scoped]
public sealed class LeagueStatsService(ILeagueStatsClient leagueStatsClient) : ILeagueStatsService
{
    public async Task<string> GetSummonerStatsAsync(string summonerName, string tag)
    {
        return await leagueStatsClient.GetSummonerStatsAsync(summonerName, tag);
    }
}
