using Newtonsoft.Json;

namespace Apotheosis.Core.Features.AiChat.Models;

public sealed class AiChatUsage
{
    /// <summary>
    /// Gets or sets the prompt token count.
    /// </summary>
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets or sets the completion token count.
    /// </summary>
    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Gets or sets the total token count.
    /// </summary>
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}