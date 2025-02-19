namespace Apotheosis.Core.Features.Recap.Exceptions;

public sealed class LeagueRecapNetworkException : Exception
{
    public LeagueRecapNetworkException()
    {
    }

    public LeagueRecapNetworkException(string message) : base(message)
    {
    }

    public LeagueRecapNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}