namespace Apotheosis.Core.Features.Stats.Interfaces;
public interface ILeagueStatsClient
{
    Task<string> GetSummonerStatsAsync(string summonerName, string tag);
}
