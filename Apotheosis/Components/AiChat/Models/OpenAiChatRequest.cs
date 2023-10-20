namespace Apotheosis.Components.AiChat.Models;

public sealed class OpenAiChatRequest
{
    public string? Model { get; set; }

    public IEnumerable<OpenAiChatMessageRequest>? Messages { get; set; }
}
