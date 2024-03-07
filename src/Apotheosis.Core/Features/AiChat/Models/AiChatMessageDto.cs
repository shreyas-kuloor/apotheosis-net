namespace Apotheosis.Core.Features.AiChat.Models;

public sealed class AiChatMessageDto
{
    /// <summary>
    /// Gets or sets the index of the chat message.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the content of the chat message.
    /// </summary>
    public string? Content { get; set; }
}