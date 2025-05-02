namespace Apotheosis.Core.Features.AiChat.Configuration;

public sealed class AiChatSettings
{
    public const string Name = "AiChat";

    /// <summary>
    /// Gets or sets the AI Chat Base URL.
    /// </summary>
    public Uri? BaseUrl { get; set; }
}