namespace Apotheosis.Server.Features.AiChat.Configuration;

public sealed class AiChatSettings
{
    public const string Name = "AiChat";

    /// <summary>
    /// Gets or sets the AI Chat Base URL.
    /// </summary>
    public Uri? Url { get; set; }
    
    /// <summary>
    /// Gets or sets the Discord ID of the AI Chat Target User (the bot).
    /// </summary>
    public ulong TargetId { get; set; }
}