namespace Apotheosis.Core.Features.AiChat.Configuration;

public sealed class AiChatSettings
{
    public const string Name = "AiChat";

    /// <summary>
    /// Gets or sets the Open AI Base URL.
    /// </summary>
    public Uri? OpenAiBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the Open AI API key.
    /// </summary>
    public string? OpenAiApiKey { get; set; }

    /// <summary>
    /// Gets or sets the Open AI model.
    /// </summary>
    public string? OpenAiModel { get; set; }

    /// <summary>
    /// Gets or sets the system instruction for AI chats.
    /// </summary>
    public string? ChatSystemInstruction { get; set; }

    /// <summary>
    /// Gets or sets the thread expiration in minutes for AI chats.
    /// </summary>
    public int ThreadExpirationMinutes { get; set; }
}