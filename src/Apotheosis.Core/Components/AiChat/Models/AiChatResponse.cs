using Newtonsoft.Json;

namespace Apotheosis.Core.Components.AiChat.Models;

public sealed class AiChatResponse
{
    /// <summary>
    /// Gets or sets the ID of the AI chat.
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the object of the AI chat.
    /// </summary>
    [JsonProperty("object")]
    public string? Object { get; set; }
    
    /// <summary>
    /// Gets or sets the created count of the AI chat.
    /// </summary>
    [JsonProperty("created")]
    public int Created { get; set; }
    
    /// <summary>
    /// Gets or sets the choices of the AI chat response.
    /// </summary>
    [JsonProperty("choices")]
    public IEnumerable<AiChatChoice>? Choices { get; set; }
    
    /// <summary>
    /// Gets or sets the usage data of the AI chat.
    /// </summary>
    [JsonProperty("usage")]
    public AiChatUsage? Usage { get; set; }
}