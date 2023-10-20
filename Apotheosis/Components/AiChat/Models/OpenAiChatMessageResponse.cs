using Newtonsoft.Json;

namespace Apotheosis.Components.AiChat.Models;

public class OpenAiChatMessageResponse
{
    public string? Role { get; set; }
    
    public string? Content { get; set; }
    
    [JsonIgnore]
    public OpenAiChatRole MessageRole => this.Role switch
    {
        "system" => OpenAiChatRole.System,
        "assistant" => OpenAiChatRole.System,
        "user" => OpenAiChatRole.User,
        _ => default
    };
}