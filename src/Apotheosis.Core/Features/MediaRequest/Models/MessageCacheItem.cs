namespace Apotheosis.Core.Features.MediaRequest.Models;
public sealed class MessageCacheItem
{
    public ulong Item { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }
}
