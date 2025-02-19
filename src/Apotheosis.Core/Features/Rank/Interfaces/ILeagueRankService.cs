using Apotheosis.Core.Features.Rank.Models;

namespace Apotheosis.Core.Features.Rank.Interfaces;
public interface ILeagueRankService
{
    Task<RankDetailsDto?> GetRankedSoloStatsAsync(string summonerName, string tag);
}
