namespace Apotheosis.Components.AiChat.Exceptions;

public class AiChatNetworkException : Exception
{
    public AiChatNetworkException()
    {
        Reason = ErrorReason.Unknown;
    }

    public AiChatNetworkException(string message) : base(message)
    {
        Reason = ErrorReason.Unknown;
    }

    public AiChatNetworkException(string message, Exception inner)
        : base(message, inner)
    {
        Reason = ErrorReason.Unknown;
    }
    
    public AiChatNetworkException(ErrorReason reason, string? message, Exception? inner)
        : base(
            message ?? $"A network error occurred when communicating with the AI Chat service. Reason: {StringFromErrorReason(reason)}", 
            inner)
    {
        Reason = reason;
    }
    
    public ErrorReason Reason { get; set; }

    private static string StringFromErrorReason(ErrorReason reason)
    {
        return reason switch
        {
            ErrorReason.TokenQuotaReached => "Token Quota Reached",
            ErrorReason.UnexpectedResponse => "Unexpected Response",
            ErrorReason.Unknown => "Unknown",
            _ => "Unknown"
        };
    }

    public enum ErrorReason
    {
        TokenQuotaReached,
        UnexpectedResponse,
        Unknown
    }
    
}