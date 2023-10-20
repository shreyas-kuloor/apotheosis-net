using Newtonsoft.Json;

namespace Apotheosis.Components.AiChat.Models;

public sealed class OpenAiChatMessageRequest
{
    [JsonIgnore]
    public OpenAiChatRole MessageRole { get; set; }
    
    public string? Role => this.MessageRole switch
    {
        OpenAiChatRole.System => "system",
        OpenAiChatRole.Assistant => "assistant",
        OpenAiChatRole.User => "user",
        _ => default
    };
    
    public string? Content { get; set; }
}
