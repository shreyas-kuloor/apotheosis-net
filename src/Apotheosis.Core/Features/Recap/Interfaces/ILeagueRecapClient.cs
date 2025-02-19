namespace Apotheosis.Core.Features.Recap.Interfaces;
public interface ILeagueRecapClient
{
    Task<string> GetSummonerRecapAsync(string summonerName, string tag);
}
