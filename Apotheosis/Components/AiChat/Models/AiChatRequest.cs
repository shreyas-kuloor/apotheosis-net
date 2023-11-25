using Newtonsoft.Json;

namespace Apotheosis.Components.AiChat.Models;

public sealed class AiChatRequest
{
    /// <summary>
    /// Gets or sets the model to use for the AI chat request.
    /// </summary>
    [JsonProperty("model")]
    public string? Model { get; set; }
    
    /// <summary>
    /// Gets or sets the messages to send with the AI chat request.
    /// </summary>
    [JsonProperty("messages")]
    public List<AiChatMessage>? Messages { get; set; }
}