namespace Apotheosis.Components.AiChat.Configuration;

public sealed class AiChatSettings
{
    /// <summary>
    /// The base URL for OpenAI chat requests.
    /// </summary>
    public Uri? OpenAiBaseUrl { get; set; }
    
    /// <summary>
    /// The model name to use for OpenAI chat requests.
    /// </summary>
    public string? OpenAiModel { get; set; }
    
    /// <summary>
    /// The API key to use for OpenAI chat requests.
    /// </summary>
    public string? OpenAiApiKey { get; set; }
    
    /// <summary>
    /// The system instruction to send to the AI chat service.
    /// </summary>
    public string? ChatSystemInstruction { get; set; }
}