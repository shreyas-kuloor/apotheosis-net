namespace Apotheosis.Components.AiChat.Models;

public sealed class MessageDto
{
    /// <summary>
    /// Gets or sets the role of the message.
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    public string? Content { get; set; }
}