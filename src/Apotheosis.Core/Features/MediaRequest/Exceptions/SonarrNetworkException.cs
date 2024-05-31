namespace Apotheosis.Core.Features.MediaRequest.Exceptions;

public sealed class SonarrNetworkException : Exception
{
    public SonarrNetworkException()
    {
    }

    public SonarrNetworkException(string message) : base(message)
    {
    }

    public SonarrNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}