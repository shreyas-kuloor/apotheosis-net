namespace Apotheosis.Core.Features.Stats.Interfaces;
public interface ILeagueStatsService
{
    Task<string> GetSummonerStatsAsync(string summonerName, string tag);
}
