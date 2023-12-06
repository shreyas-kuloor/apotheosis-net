namespace Apotheosis.Core.Components.AiChat.Models;

public sealed class ThreadChannelDto
{
    /// <summary>
    /// Gets or sets the thread ID of the thread channel.
    /// </summary>
    public ulong ThreadId { get; set; }
    
    /// <summary>
    /// Gets or sets the expiration of the thread channel.
    /// </summary>
    public DateTimeOffset Expiration { get; set; }
    
    /// <summary>
    /// Gets or sets the initial message content of the thread channel.
    /// </summary>
    public string? InitialMessageContent { get; set; }
}