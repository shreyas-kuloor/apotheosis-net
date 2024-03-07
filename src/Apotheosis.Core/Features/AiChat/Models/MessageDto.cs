namespace Apotheosis.Core.Features.AiChat.Models;

public sealed class MessageDto
{
    /// <summary>
    /// Gets or sets the content of the thread message.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message is from the bot.
    /// </summary>
    public bool IsBot { get; set; }
}