namespace Apotheosis.Core.Features.MediaRequest.Exceptions;

public sealed class RadarrNetworkException : Exception
{
    public RadarrNetworkException()
    {
    }

    public RadarrNetworkException(string message) : base(message)
    {
    }

    public RadarrNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}