using Newtonsoft.Json;

namespace Apotheosis.Components.AiChat.Models;

public sealed class AiChatChoice
{
    /// <summary>
    /// Gets or sets the index of the chat choice.
    /// </summary>
    [JsonProperty("index")]
    public int Index { get; set; }
    
    /// <summary>
    /// Gets or sets the message of the chat choice.
    /// </summary>
    [JsonProperty("message")]
    public AiChatMessage? Message { get; set; }
    
    /// <summary>
    /// Gets or sets the finish reason of the chat choice.
    /// </summary>
    [JsonProperty("finish_reason")]
    public string? FinishReason { get; set; }
}