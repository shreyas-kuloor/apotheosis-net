namespace Apotheosis.Core.Features.Recap.Interfaces;
public interface ILeagueRecapService
{
    Task<string> GetSummonerRecapAsync(string summonerName, string tag);
}
