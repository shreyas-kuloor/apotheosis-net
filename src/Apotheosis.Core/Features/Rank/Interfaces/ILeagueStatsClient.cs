using Apotheosis.Core.Features.Rank.Models;

namespace Apotheosis.Core.Features.Rank.Interfaces;
public interface ILeagueStatsClient
{
    Task<IEnumerable<SummonerStatsDto>> GetSummonerStatsAsync(string summonerName, string tag);
}
