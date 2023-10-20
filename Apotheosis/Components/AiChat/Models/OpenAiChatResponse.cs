namespace Apotheosis.Components.AiChat.Models;

public sealed class OpenAiChatResponse
{
    public string? Id { get; set; }
    
    public string? Object { get; set; }
    
    public int Created { get; set; }
    
    public IEnumerable<OpenAiChatChoiceResponse>? Choices { get; set; }
    
    public OpenAiChatUsageResponse? Usage { get; set; }
}