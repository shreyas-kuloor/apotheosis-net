namespace Apotheosis.Server.Features.Rank.Exceptions;

public sealed class LeagueRankNetworkException : Exception
{
    public LeagueRankNetworkException()
    {
    }

    public LeagueRankNetworkException(string message) : base(message)
    {
    }

    public LeagueRankNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}