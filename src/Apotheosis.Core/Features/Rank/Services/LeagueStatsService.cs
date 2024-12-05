using Apotheosis.Core.Features.Rank.Interfaces;
using Apotheosis.Core.Features.Rank.Models;
using Apotheosis.Core.Features.Rank.SmartEnums;

namespace Apotheosis.Core.Features.Rank.Services;

[Scoped]
public sealed class LeagueStatsService(ILeagueStatsClient leagueStatsClient) : ILeagueStatsService
{
    public async Task<RankDetailsDto?> GetRankedSoloStatsAsync(string summonerName, string tag)
    {
        var summonerStats = await leagueStatsClient.GetSummonerStatsAsync(summonerName, tag);

        var rankedSoloStats = summonerStats.Select(s => new RankDetailsDto
        {
            QueueType = QueueType.FromValue(s.QueueType ?? string.Empty, QueueType.Unknown),
            Tier = s.Tier,
            Rank = s.Rank,
            LeaguePoints = s.LeaguePoints,
            Wins = s.Wins,
            Losses = s.Losses,
        });

        return rankedSoloStats.FirstOrDefault(r => r.QueueType == QueueType.RankedSoloDuo);
    }
}
