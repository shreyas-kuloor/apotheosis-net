namespace Apotheosis.Core.Features.GcpDot.Exceptions;

public sealed class GcpDotNetworkException : Exception
{
    public GcpDotNetworkException()
    {
    }

    public GcpDotNetworkException(string message) : base(message)
    {
    }

    public GcpDotNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}