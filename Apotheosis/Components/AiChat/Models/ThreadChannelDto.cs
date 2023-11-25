namespace Apotheosis.Components.AiChat.Models;

public class ThreadChannelDto
{
    /// <summary>
    /// Gets or sets the thread ID of the thread channel.
    /// </summary>
    public ulong ThreadId { get; set; }
    
    /// <summary>
    /// Gets or sets the expiration of the thread channel.
    /// </summary>
    public DateTimeOffset Expiration { get; set; }
}