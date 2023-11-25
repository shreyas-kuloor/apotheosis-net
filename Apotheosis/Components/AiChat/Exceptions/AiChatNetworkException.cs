namespace Apotheosis.Components.AiChat.Exceptions;

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
    
    public AiChatNetworkException(string? message, ErrorReason reason, Exception? inner)
        : base(message, inner)
    {
        Reason = reason;
    }
    
    public ErrorReason Reason { get; set; }
    
    public enum ErrorReason
    {
        TokenQuotaReached,
        Unsuccessful,
        Unknown,
    }
}