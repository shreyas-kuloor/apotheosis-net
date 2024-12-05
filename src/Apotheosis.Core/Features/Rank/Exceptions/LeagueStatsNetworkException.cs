namespace Apotheosis.Core.Features.Rank.Exceptions;

public sealed class LeagueStatsNetworkException : Exception
{
    public LeagueStatsNetworkException()
    {
    }

    public LeagueStatsNetworkException(string message) : base(message)
    {
    }

    public LeagueStatsNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}