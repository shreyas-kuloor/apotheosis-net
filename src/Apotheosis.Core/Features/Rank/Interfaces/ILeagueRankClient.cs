using Apotheosis.Core.Features.Rank.Models;

namespace Apotheosis.Core.Features.Rank.Interfaces;
public interface ILeagueRankClient
{
    Task<IEnumerable<SummonerRankStatsDto>> GetSummonerRankStatsAsync(string summonerName, string tag);
}
