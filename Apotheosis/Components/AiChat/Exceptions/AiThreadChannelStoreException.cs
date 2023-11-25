namespace Apotheosis.Components.AiChat.Exceptions;

public sealed class AiThreadChannelStoreException : Exception
{
    public AiThreadChannelStoreException()
    {
    }

    public AiThreadChannelStoreException(string message) : base(message)
    {
    }

    public AiThreadChannelStoreException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}