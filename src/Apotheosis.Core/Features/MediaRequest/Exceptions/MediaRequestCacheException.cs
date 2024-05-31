namespace Apotheosis.Core.Features.MediaRequest.Exceptions;

public sealed class MediaRequestCacheException : Exception
{
    public MediaRequestCacheException()
    {
    }

    public MediaRequestCacheException(string message) : base(message)
    {
    }

    public MediaRequestCacheException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}