using Apotheosis.Core.Features.Recap.Interfaces;

namespace Apotheosis.Core.Features.Recap.Services;

[Scoped]
public sealed class LeagueRecapService(ILeagueRecapClient leagueStatsClient) : ILeagueRecapService
{
    public async Task<string> GetSummonerRecapAsync(string summonerName, string tag)
    {
        return await leagueStatsClient.GetSummonerRecapAsync(summonerName, tag);
    }
}
