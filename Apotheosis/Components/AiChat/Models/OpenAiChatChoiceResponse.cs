using Newtonsoft.Json;

namespace Apotheosis.Components.AiChat.Models;

public sealed class OpenAiChatChoiceResponse
{
    public int Index { get; set; }
    
    public OpenAiChatMessageResponse? Message { get; set; }
    
    [JsonProperty("finish_reason")]
    public string? FinishReason { get; set; }
}