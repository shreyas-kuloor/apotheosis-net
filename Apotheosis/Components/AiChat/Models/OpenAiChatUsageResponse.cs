using Newtonsoft.Json;

namespace Apotheosis.Components.AiChat.Models;

public sealed class OpenAiChatUsageResponse
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }
    
    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }
    
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}