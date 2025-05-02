namespace Apotheosis.Core.Features.AiChat.Exceptions;

public sealed class AiChatNetworkException : Exception
{
    public AiChatNetworkException()
    {
    }

    public AiChatNetworkException(string message) : base(message)
    {
    }

    public AiChatNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}